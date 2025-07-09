namespace Particular.Obsoletes.Fixes;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ObsoleteCodeFixProvider))]
public class ObsoleteCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        [
            DiagnosticIds.MissingObsoleteAttribute,
            DiagnosticIds.ObsoleteAttributeMissingConstructorArguments,
            DiagnosticIds.IncorrectObsoleteAttributeMessageArgument,
            DiagnosticIds.IncorrectObsoleteAttributeIsErrorArgument
        ];

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        foreach (var diagnostic in context.Diagnostics)
        {
            if (diagnostic.Id == DiagnosticIds.MissingObsoleteAttribute)
            {
                diagnostic.Properties.TryGetValue("Message", out var message);
                diagnostic.Properties.TryGetValue("IsError", out var isError);

                var codeAction = CodeAction.Create("Add missing Obsolete attribute", token => AddMissingObsoleteAttribute(context.Document, diagnostic.Location, message ?? string.Empty, isError ?? string.Empty, token), "Add missing Obsolete attribute");

                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else if (diagnostic.Id == DiagnosticIds.ObsoleteAttributeMissingConstructorArguments)
            {
                diagnostic.Properties.TryGetValue("Message", out var message);
                diagnostic.Properties.TryGetValue("IsError", out var isError);

                var codeAction = CodeAction.Create("Add missing constructor arguments", token => AddMissingConstructorArguments(context.Document, diagnostic.Location, message ?? string.Empty, isError ?? string.Empty, token), "Add missing constructor arguments");

                context.RegisterCodeFix(codeAction, diagnostic);
            }
        }

        return Task.CompletedTask;
    }

    static async Task<Document> AddMissingObsoleteAttribute(Document document, Location location, string message, string isError, CancellationToken cancellationToken)
    {
        if (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is not SyntaxNode root)
        {
            return document;
        }

        if (root.FindNode(location.SourceSpan) is not SyntaxNode { Parent.Parent: SyntaxNode member })
        {
            return document;
        }

        if (await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false) is not SemanticModel semanticModel)
        {
            return document;
        }

        if (semanticModel.Compilation.GetTypeByMetadataName("System.ObsoleteAttribute") is not INamedTypeSymbol obsoleteAttributeTypeSymbol)
        {
            return document;
        }

        var generator = SyntaxGenerator.GetGenerator(document);

        var obsoleteAttributeTypeNode = generator.TypeExpression(obsoleteAttributeTypeSymbol).WithAdditionalAnnotations(Simplifier.AddImportsAnnotation);
        var obsoleteAttributeNode = generator.Attribute(obsoleteAttributeTypeNode, [generator.AttributeArgument(generator.LiteralExpression(message)), generator.AttributeArgument(generator.LiteralExpression(bool.Parse(isError)))]);

        var newMemberNode = generator.AddAttributes(member, obsoleteAttributeNode);
        var newRoot = generator.ReplaceNode(root, member, newMemberNode);
        var newDocument = document.WithSyntaxRoot(newRoot);

        return newDocument;
    }

    static async Task<Document> AddMissingConstructorArguments(Document document, Location location, string message, string isError, CancellationToken cancellationToken)
    {
        if (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is not SyntaxNode root)
        {
            return document;
        }

        if (root.FindNode(location.SourceSpan) is not AttributeSyntax original)
        {
            return document;
        }

        if (await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false) is not SemanticModel semanticModel)
        {
            return document;
        }

        if (semanticModel.Compilation.GetTypeByMetadataName("System.ObsoleteAttribute") is not INamedTypeSymbol obsoleteAttributeTypeSymbol)
        {
            return document;
        }

        var generator = SyntaxGenerator.GetGenerator(document);

        var obsoleteAttributeTypeNode = generator.TypeExpression(obsoleteAttributeTypeSymbol).WithAdditionalAnnotations(Simplifier.AddImportsAnnotation);
        var obsoleteAttributeNode = generator.Attribute(obsoleteAttributeTypeNode, [generator.AttributeArgument(generator.LiteralExpression(message)), generator.AttributeArgument(generator.LiteralExpression(bool.Parse(isError)))]);

        var newRoot = generator.ReplaceNode(root, original, obsoleteAttributeNode);
        var newDocument = document.WithSyntaxRoot(newRoot);

        return newDocument;
    }
}
