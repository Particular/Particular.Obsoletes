namespace Particular.Obsoletes;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ObsoleteExAnalyzer : DiagnosticAnalyzer
{
    static readonly ImmutableArray<SyntaxKind> syntaxKinds = [SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.EnumDeclaration, SyntaxKind.ConstructorDeclaration, SyntaxKind.MethodDeclaration, SyntaxKind.PropertyDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.EventDeclaration, SyntaxKind.DelegateDeclaration];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [];

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
            AttributeSyntax? obsoleteEx = null;
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

                    if (nameToken.Value.ValueText is "ObsoleteEx")
                    {
                        obsoleteEx = attributeSyntax;
                    }
                    else if (nameToken.Value.ValueText is "Obsolete")
                    {
                        obsolete = attributeSyntax;
                    }
                }
            }

            if (obsoleteEx is not null || obsolete is not null)
            {
                Analyze(context, obsoleteEx, obsolete);
            }
        }
    }

    static void Analyze(SyntaxNodeAnalysisContext context, AttributeSyntax? obsoleteEx, AttributeSyntax? obsolete)
    {
        _ = context;
        _ = obsoleteEx;
        _ = obsolete;

        //if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
        //{
        //    continue;
        //}

        //var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
        //var fullName = attributeContainingTypeSymbol.ToDisplayString();

        //if (fullName == "Particular.Obsoletes.ObsoleteExAttribute")
        //{
        //    obsoleteEx = attributeSymbol;
        //}
        //else if (fullName == "System.ObsoleteAttribute")
        //{
        //    obsolete = attributeSymbol;
        //}
    }

}
