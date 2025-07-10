namespace Particular.Obsoletes;

using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ObsoleteAnalyzer : DiagnosticAnalyzer
{
    static readonly ImmutableArray<SyntaxKind> syntaxKinds =
        [
            SyntaxKind.ClassDeclaration,
            SyntaxKind.StructDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.EnumDeclaration,
            SyntaxKind.ConstructorDeclaration,
            SyntaxKind.MethodDeclaration,
            SyntaxKind.PropertyDeclaration,
            SyntaxKind.FieldDeclaration,
            SyntaxKind.EventDeclaration,
            SyntaxKind.DelegateDeclaration
        ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [
            DiagnosticDescriptors.MissingObsoleteMetadataAttribute,
            DiagnosticDescriptors.MissingObsoleteAttribute,
            DiagnosticDescriptors.InvalidTreatAsErrorFromVersion,
            DiagnosticDescriptors.InvalidRemoveInVersion,
            DiagnosticDescriptors.RemoveInVersionLessThanOrEqualToTreatAsErrorFromVersion,
            DiagnosticDescriptors.RemoveObsoleteMember,
            DiagnosticDescriptors.ObsoleteAttributeMissingConstructorArguments,
            DiagnosticDescriptors.IncorrectObsoleteAttributeMessageArgument,
            DiagnosticDescriptors.IncorrectObsoleteAttributeIsErrorArgument,
            DiagnosticDescriptors.MissingTreatAsErrorFromVersion,
            DiagnosticDescriptors.MissingRemoveInVersion
        ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSyntax, syntaxKinds);
    }

    static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is MemberDeclarationSyntax member && member.AttributeLists.Any())
        {
            AttributeSyntax? obsoleteMetadata = null;
            AttributeSyntax? obsolete = null;

            foreach (var attributeListSyntax in member.AttributeLists)
            {
                foreach (var attributeSyntax in attributeListSyntax.Attributes)
                {
                    SyntaxToken? nameToken = attributeSyntax.Name switch
                    {
                        IdentifierNameSyntax name => name.Identifier,
                        QualifiedNameSyntax name => name.Right.Identifier,
                        _ => null
                    };

                    if (nameToken is null)
                    {
                        continue;
                    }

                    if (nameToken.Value.ValueText is "ObsoleteMetadata")
                    {
                        obsoleteMetadata = attributeSyntax;
                    }
                    else if (nameToken.Value.ValueText is "Obsolete")
                    {
                        obsolete = attributeSyntax;
                    }
                }
            }

            if (obsoleteMetadata is not null || obsolete is not null)
            {
                Analyze(context, member, obsoleteMetadata?.ArgumentList?.Arguments, obsolete?.ArgumentList?.Arguments);
            }
        }
    }

    static void Analyze(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax memberDeclarationSyntax, SeparatedSyntaxList<AttributeArgumentSyntax>? obsoleteMetadataAttributeArguments, SeparatedSyntaxList<AttributeArgumentSyntax>? obsoleteAttributeArguments)
    {
        if (context.SemanticModel.GetDeclaredSymbol(memberDeclarationSyntax) is not ISymbol symbol)
        {
            return;
        }

        var (obsoleteMetadataAttribute, obsoleteAttribute) = GetAttributeData(context, symbol);

        if (obsoleteMetadataAttribute is null)
        {
            if (obsoleteAttribute is not null)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MissingObsoleteMetadataAttribute, CreateLocation(obsoleteAttribute.ApplicationSyntaxReference)));
            }

            return;
        }

        string? message = null;
        string? treatAsErrorFromVersionValue = null;
        bool treatAsErrorFromVersionSet = false;
        string? removeInVersionValue = null;
        bool removeInVersionSet = false;
        string? replacementTypeOrMember = null;

        foreach (var argument in obsoleteMetadataAttribute.NamedArguments)
        {
            if (argument.Key == "Message")
            {
                message = argument.Value.Value?.ToString();
            }
            else if (argument.Key == "TreatAsErrorFromVersion")
            {
                treatAsErrorFromVersionValue = argument.Value.Value?.ToString();
                treatAsErrorFromVersionSet = true;
            }
            else if (argument.Key == "RemoveInVersion")
            {
                removeInVersionValue = argument.Value.Value?.ToString();
                removeInVersionSet = true;
            }
            else if (argument.Key == "ReplacementTypeOrMember")
            {
                replacementTypeOrMember = argument.Value.Value?.ToString();
            }
        }

        if (!treatAsErrorFromVersionSet)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MissingTreatAsErrorFromVersion, CreateLocation(obsoleteMetadataAttribute.ApplicationSyntaxReference), treatAsErrorFromVersionValue));
        }

        if (!removeInVersionSet)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MissingRemoveInVersion, CreateLocation(obsoleteMetadataAttribute.ApplicationSyntaxReference), removeInVersionValue));
        }

        if (!treatAsErrorFromVersionSet || !removeInVersionSet)
        {
            return;
        }

        if (!TryParseVersion(treatAsErrorFromVersionValue, out var treatAsErrorFromVersion))
        {
            var attributeArgument = GetAttributeArgument(obsoleteMetadataAttributeArguments, "TreatAsErrorFromVersion");
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.InvalidTreatAsErrorFromVersion, CreateLocation(attributeArgument), treatAsErrorFromVersionValue));
        }

        if (!TryParseVersion(removeInVersionValue, out var removeInVersion))
        {
            var attributeArgument = GetAttributeArgument(obsoleteMetadataAttributeArguments, "RemoveInVersion");
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.InvalidRemoveInVersion, CreateLocation(attributeArgument), removeInVersionValue));
        }

        if (treatAsErrorFromVersion is null || removeInVersion is null)
        {
            return;
        }

        if (removeInVersion <= treatAsErrorFromVersion)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.RemoveInVersionLessThanOrEqualToTreatAsErrorFromVersion, CreateLocation(obsoleteMetadataAttribute.ApplicationSyntaxReference), removeInVersion, treatAsErrorFromVersion));
            return;
        }

        var assemblyVersion = context.Compilation.Assembly.Identity.Version;

        if (assemblyVersion.Major == 1)
        {
            var assemblyMetadataAttributeType = context.Compilation.GetTypeByMetadataName("System.Reflection.AssemblyMetadataAttribute");
            var assemblyAttributes = context.Compilation.Assembly.GetAttributes();

            foreach (var assemblyAttribute in assemblyAttributes)
            {
                if (SymbolEqualityComparer.Default.Equals(assemblyAttribute.AttributeClass, assemblyMetadataAttributeType) && assemblyAttribute.ConstructorArguments.Length == 2)
                {
                    if (assemblyAttribute.ConstructorArguments[0].Value?.ToString() == "MajorMinorPatch" && assemblyAttribute.ConstructorArguments[1].Value?.ToString() == "..")
                    {
                        return;
                    }
                }
            }
        }

        if (assemblyVersion >= removeInVersion)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.RemoveObsoleteMember, CreateLocation(memberDeclarationSyntax.SyntaxTree, memberDeclarationSyntax.Span), assemblyVersion, removeInVersion));
            return;
        }

        var expectedObsoleteMessage = BuildMessage(assemblyVersion, message, replacementTypeOrMember, treatAsErrorFromVersion, removeInVersion);
        var expectedIsError = treatAsErrorFromVersion is not null && assemblyVersion >= treatAsErrorFromVersion;

        var properties = new Dictionary<string, string?>
        {
            { "Message", expectedObsoleteMessage },
            { "IsError", expectedIsError.ToString() },
        }.ToImmutableDictionary();

        if (obsoleteAttribute is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MissingObsoleteAttribute, CreateLocation(obsoleteMetadataAttribute.ApplicationSyntaxReference), properties));
            return;
        }

        if (obsoleteAttribute.ConstructorArguments.Length != 2)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.ObsoleteAttributeMissingConstructorArguments, CreateLocation(obsoleteAttribute.ApplicationSyntaxReference), properties, obsoleteAttribute.ConstructorArguments.Length));
            return;
        }

        var obsoleteMessage = obsoleteAttribute.ConstructorArguments[0].Value?.ToString();
        var isError = (bool)(obsoleteAttribute.ConstructorArguments[1].Value ?? false);

        if (obsoleteMessage != expectedObsoleteMessage)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.IncorrectObsoleteAttributeMessageArgument, CreateLocation(obsoleteAttributeArguments?[0]), properties));
        }

        if (isError != expectedIsError)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.IncorrectObsoleteAttributeIsErrorArgument, CreateLocation(obsoleteAttributeArguments?[1]), properties));
        }
    }

    static (AttributeData? obsoleteMetadataAttribute, AttributeData? obsoleteAttribute) GetAttributeData(SyntaxNodeAnalysisContext context, ISymbol symbol)
    {
        AttributeData? obsoleteMetadataAttribute = null;
        AttributeData? obsoleteAttribute = null;

        var obsoleteMetadataAttributeType = context.Compilation.GetTypeByMetadataName("Particular.Obsoletes.ObsoleteMetadataAttribute");
        var obsoleteAttributeType = context.Compilation.GetTypeByMetadataName("System.ObsoleteAttribute");

        foreach (var attribute in symbol.GetAttributes())
        {
            if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, obsoleteMetadataAttributeType))
            {
                obsoleteMetadataAttribute = attribute;
            }
            else if (SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, obsoleteAttributeType))
            {
                obsoleteAttribute = attribute;
            }
        }

        return (obsoleteMetadataAttribute, obsoleteAttribute);
    }

    static Location CreateLocation(SyntaxReference? syntaxReference) => CreateLocation(syntaxReference?.SyntaxTree, syntaxReference?.Span);

    static Location CreateLocation(AttributeArgumentSyntax? attributeArgumentSyntax) => CreateLocation(attributeArgumentSyntax?.SyntaxTree, attributeArgumentSyntax?.Span);

    static Location CreateLocation(SyntaxTree? syntaxTree, TextSpan? textSpan)
    {
        var location = Location.None;

        if (syntaxTree is not null && textSpan is not null)
        {
            location = Location.Create(syntaxTree, textSpan.Value);
        }

        return location;
    }

    static bool TryParseVersion(string? input, out Version? result)
    {
        if (input?.IndexOf('.') == -1 && int.TryParse(input, out var major))
        {
            result = new Version(major, 0, 0);
            return true;
        }

        return Version.TryParse(input, out result);
    }

    static AttributeArgumentSyntax? GetAttributeArgument(SeparatedSyntaxList<AttributeArgumentSyntax>? attributeArguments, string argumentName)
    {
        AttributeArgumentSyntax? attributeArgument = null;

        if (attributeArguments is not null)
        {
            foreach (var argument in attributeArguments)
            {
                if (argument.NameEquals?.Name.Identifier.ValueText == argumentName)
                {
                    attributeArgument = argument;
                    break;
                }
            }
        }

        return attributeArgument;
    }

    static string BuildMessage(Version assemblyVersion, string? message, string? replacementTypeOrMember, Version treatAsErrorFromVersion, Version removeInVersion)
    {
        var builder = new StringBuilder();

        if (message is not null)
        {
            builder.AppendFormat("{0}. ", message);
        }

        if (replacementTypeOrMember is not null)
        {
            builder.AppendFormat("Use '{0}' instead. ", replacementTypeOrMember);
        }

        if (assemblyVersion < treatAsErrorFromVersion)
        {
            var treatAsErrorFromVersionPatch = treatAsErrorFromVersion.Build == -1 ? 0 : treatAsErrorFromVersion.Build;
            builder.AppendFormat("Will be treated as an error from version {0}.{1}.{2}. ", treatAsErrorFromVersion.Major, treatAsErrorFromVersion.Minor, treatAsErrorFromVersionPatch);
        }

        var removeInVersionPatch = removeInVersion.Build == -1 ? 0 : removeInVersion.Build;
        builder.AppendFormat("Will be removed in version {0}.{1}.{2}.", removeInVersion.Major, removeInVersion.Minor, removeInVersionPatch);

        return builder.ToString();
    }
}