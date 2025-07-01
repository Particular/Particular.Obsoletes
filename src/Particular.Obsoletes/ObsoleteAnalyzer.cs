namespace Particular.Obsoletes;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ObsoleteAnalyzer : DiagnosticAnalyzer
{
    static readonly ImmutableArray<SyntaxKind> syntaxKinds = [SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.EnumDeclaration, SyntaxKind.ConstructorDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.EventDeclaration, SyntaxKind.DelegateDeclaration];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [DiagnosticDescriptors.MissingObsoleteMetadataAttribute, DiagnosticDescriptors.MissingObsoleteAttribute, DiagnosticDescriptors.RemoveObsoleteMember];

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
                Analyze(context, member);
            }
        }
    }

    static void Analyze(SyntaxNodeAnalysisContext context, MemberDeclarationSyntax memberDeclarationSyntax)
    {
        var symbol = context.SemanticModel.GetDeclaredSymbol(memberDeclarationSyntax);

        if (symbol is null)
        {
            return;
        }

        var obsoleteMetadataAttributeType = context.Compilation.GetTypeByMetadataName("Particular.Obsoletes.ObsoleteMetadataAttribute");
        var obsoleteAttributeType = context.Compilation.GetTypeByMetadataName("System.ObsoleteAttribute");

        AttributeData? obsoleteMetadataAttribute = null;
        AttributeData? obsoleteAttribute = null;

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

        if (obsoleteMetadataAttribute is null && obsoleteAttribute is null)
        {
            return;
        }

        if (obsoleteAttribute is not null && obsoleteMetadataAttribute is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MissingObsoleteMetadataAttribute, CreateLocation(obsoleteAttribute.ApplicationSyntaxReference)));
            return;
        }

        if (obsoleteMetadataAttribute is not null && obsoleteAttribute is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MissingObsoleteAttribute, CreateLocation(obsoleteMetadataAttribute.ApplicationSyntaxReference)));
        }

        string? message = null;
        string? treatAsErrorFromVersion = null;
        string? removeInVersion = null;
        string? replacementTypeOrMember = null;

        foreach (var argument in obsoleteMetadataAttribute.NamedArguments)
        {
            if (argument.Key == "Message")
            {
                message = argument.Value.Value?.ToString();
            }
            else if (argument.Key == "TreatAsErrorFromVersion")
            {
                treatAsErrorFromVersion = argument.Value.Value?.ToString();
            }
            else if (argument.Key == "RemoveInVersion")
            {
                removeInVersion = argument.Value.Value?.ToString();
            }
            else if (argument.Key == "ReplacementTypeOrMember")
            {
                replacementTypeOrMember = argument.Value.Value?.ToString();
            }
        }

        _ = message;
        _ = treatAsErrorFromVersion;
        _ = replacementTypeOrMember;

        Version.TryParse(removeInVersion, out var result);
        var assemblyVersion = context.Compilation.Assembly.Identity.Version;

        if (assemblyVersion >= result)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.RemoveObsoleteMember, CreateLocation(obsoleteMetadataAttribute.ApplicationSyntaxReference), assemblyVersion, result));
        }

    }

    static Location? CreateLocation(SyntaxReference? syntaxReference)
    {
        Location? location = null;

        if (syntaxReference is not null)
        {
            location = Location.Create(syntaxReference.SyntaxTree, syntaxReference.Span);
        }

        return location;

    }
}