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
            diagnostic.Properties.TryGetValue("Message", out var message);
            diagnostic.Properties.TryGetValue("IsError", out var isError);

            message ??= string.Empty;
            isError ??= string.Empty;

            if (diagnostic.Id == DiagnosticIds.MissingObsoleteAttribute)
            {
                var title = "Add missing Obsolete attribute";
                var codeAction = CodeAction.Create(title, token => AddMissingObsoleteAttribute(context.Document, diagnostic.Location, message, isError, token), title);
                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else if (diagnostic.Id == DiagnosticIds.ObsoleteAttributeMissingConstructorArguments)
            {
                var title = "Add missing constructor arguments";
                var codeAction = CodeAction.Create(title, token => AddMissingConstructorArguments(context.Document, diagnostic.Location, message, isError, token), title);
                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else if (diagnostic.Id == DiagnosticIds.IncorrectObsoleteAttributeMessageArgument)
            {
                var title = "Fix incorrect message argument";
                var codeAction = CodeAction.Create(title, token => FixIncorrectObsoleteAttributeMessageArgument(context.Document, diagnostic.Location, message, token), title);
                context.RegisterCodeFix(codeAction, diagnostic);
            }
            else if (diagnostic.Id == DiagnosticIds.IncorrectObsoleteAttributeIsErrorArgument)
            {
                var title = "Fix incorrect isError argument";
                var codeAction = CodeAction.Create(title, token => FixIncorrectObsoleteAttributeIsErrorArgument(context.Document, diagnostic.Location, isError, token), title);
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

    static Task<Document> FixIncorrectObsoleteAttributeMessageArgument(Document document, Location location, string message, CancellationToken cancellationToken) => FixIncorrectObsoleteAttributeArgument(document, location, generator => generator.LiteralExpression(message), cancellationToken);

    static Task<Document> FixIncorrectObsoleteAttributeIsErrorArgument(Document document, Location location, string isError, CancellationToken cancellationToken) => FixIncorrectObsoleteAttributeArgument(document, location, generator => generator.LiteralExpression(bool.Parse(isError)), cancellationToken);

    static async Task<Document> FixIncorrectObsoleteAttributeArgument(Document document, Location location, Func<SyntaxGenerator, SyntaxNode> literalExpression, CancellationToken cancellationToken)
    {
        if (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is not SyntaxNode root)
        {
            return document;
        }

        if (root.FindNode(location.SourceSpan) is not AttributeArgumentSyntax original)
        {
            return document;
        }

        var generator = SyntaxGenerator.GetGenerator(document);

        var newArgument = generator.AttributeArgument(literalExpression(generator));
        var newRoot = generator.ReplaceNode(root, original, newArgument);
        var newDocument = document.WithSyntaxRoot(newRoot);

        return newDocument;
    }
}
