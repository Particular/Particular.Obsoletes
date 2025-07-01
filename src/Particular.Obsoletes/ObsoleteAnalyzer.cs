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

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [DiagnosticDescriptors.MissingObsoleteMetadataAttribute, DiagnosticDescriptors.MissingObsoleteAttribute];

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
                Analyze(context, obsoleteMetadata, obsolete);
            }
        }
    }

    static void Analyze(SyntaxNodeAnalysisContext context, AttributeSyntax? obsoleteMetadataSyntax, AttributeSyntax? obsoleteSyntax)
    {
        var obsoleteMetadataSymbol = GetMethodSymbol(context, obsoleteMetadataSyntax, "Particular.Obsoletes.ObsoleteMetadataAttribute");
        var obsoleteSymbol = GetMethodSymbol(context, obsoleteSyntax, "System.ObsoleteAttribute");

        if (obsoleteMetadataSymbol is null && obsoleteSymbol is null)
        {
            return;
        }

        if (obsoleteSymbol is not null && obsoleteMetadataSymbol is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MissingObsoleteMetadataAttribute, obsoleteSyntax?.GetLocation()));
            return;
        }

        if (obsoleteMetadataSymbol is not null && obsoleteSymbol is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MissingObsoleteAttribute, obsoleteMetadataSyntax?.GetLocation()));
            return;
        }
    }

    static IMethodSymbol? GetMethodSymbol(SyntaxNodeAnalysisContext context, AttributeSyntax? attributeSyntax, string typeName)
    {
        IMethodSymbol? symbol = null;

        if (attributeSyntax is not null)
        {
            symbol = context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol as IMethodSymbol;
        }

        if (symbol is not null)
        {
            var fullName = symbol.ContainingType.ToDisplayString();

            if (fullName != typeName)
            {
                symbol = null;
            }
        }

        return symbol;
    }

}
