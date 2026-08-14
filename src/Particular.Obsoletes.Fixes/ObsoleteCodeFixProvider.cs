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
            DiagnosticIds.IncorrectObsoleteAttributeErrorArgument,
            DiagnosticIds.IncorrectObsoleteAttributeDiagnosticIdArgument,
            DiagnosticIds.IncorrectObsoleteAttributeUrlFormatArgument,
            DiagnosticIds.MissingObsoleteAttributeDiagnosticIdArgument,
            DiagnosticIds.MissingObsoleteAttributeUrlFormatArgument
        ];

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        foreach (var diagnostic in context.Diagnostics)
        {
            diagnostic.Properties.TryGetValue("Message", out var message);
            diagnostic.Properties.TryGetValue("Error", out var error);
            diagnostic.Properties.TryGetValue("DiagnosticId", out var diagnosticId);
            diagnostic.Properties.TryGetValue("UrlFormat", out var urlFormat);

            message ??= string.Empty;
            error ??= string.Empty;

            switch (diagnostic.Id)
            {
                case DiagnosticIds.MissingObsoleteAttribute:
                    {
                        var title = "Add missing Obsolete attribute";
                        var codeAction = CodeAction.Create(title, token => AddMissingObsoleteAttribute(context.Document, diagnostic.Location, message, error, diagnosticId, urlFormat, token), title);
                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIds.ObsoleteAttributeMissingConstructorArguments:
                    {
                        var title = "Add missing attribute arguments";
                        var codeAction = CodeAction.Create(title, token => AddMissingConstructorArguments(context.Document, diagnostic.Location, message, error, diagnosticId, urlFormat, token), title);
                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIds.IncorrectObsoleteAttributeMessageArgument:
                    {
                        var title = "Fix incorrect message argument";
                        var codeAction = CodeAction.Create(title, token => FixIncorrectObsoleteAttributeMessageArgument(context.Document, diagnostic.Location, message, token), title);
                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIds.IncorrectObsoleteAttributeErrorArgument:
                    {
                        var title = "Fix incorrect error argument";
                        var codeAction = CodeAction.Create(title, token => FixIncorrectObsoleteAttributeErrorArgument(context.Document, diagnostic.Location, error, token), title);
                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIds.IncorrectObsoleteAttributeDiagnosticIdArgument:
                    {
                        var title = "Fix incorrect DiagnosticId argument";
                        var codeAction = CodeAction.Create(title, token => FixIncorrectObsoleteAttributeDiagnosticIdArgument(context.Document, diagnostic.Location, diagnosticId, token), title);
                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIds.IncorrectObsoleteAttributeUrlFormatArgument:
                    {
                        var title = "Fix incorrect UrlFormat argument";
                        var codeAction = CodeAction.Create(title, token => FixIncorrectObsoleteAttributeUrlFormatArgument(context.Document, diagnostic.Location, urlFormat, token), title);
                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIds.MissingObsoleteAttributeDiagnosticIdArgument:
                    {
                        var title = "Add missing DiagnosticId argument";
                        var codeAction = CodeAction.Create(title, token => FixIncorrectObsoleteAttributeDiagnosticIdArgument(context.Document, diagnostic.Location, diagnosticId, token), title);
                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }
                case DiagnosticIds.MissingObsoleteAttributeUrlFormatArgument:
                    {
                        var title = "Add missing UrlFormat argument";
                        var codeAction = CodeAction.Create(title, token => FixIncorrectObsoleteAttributeUrlFormatArgument(context.Document, diagnostic.Location, urlFormat, token), title);
                        context.RegisterCodeFix(codeAction, diagnostic);
                        break;
                    }

                default:
                    break;
            }
        }

        return Task.CompletedTask;
    }

    static async Task<Document> AddMissingObsoleteAttribute(Document document, Location location, string message, string error, string? diagnosticId, string? urlFormat, CancellationToken cancellationToken)
    {
        if (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is not { } root ||
            root.FindNode(location.SourceSpan) is not { Parent.Parent: { } member } ||
            await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false) is not { } semanticModel
            || semanticModel.Compilation.GetTypeByMetadataName("System.ObsoleteAttribute") is not { } obsoleteAttributeTypeSymbol)
        {
            return document;
        }

        var generator = SyntaxGenerator.GetGenerator(document);

        var obsoleteAttributeTypeNode = generator.TypeExpression(obsoleteAttributeTypeSymbol).WithAdditionalAnnotations(Simplifier.AddImportsAnnotation);
        var obsoleteAttributeNode = generator.Attribute(obsoleteAttributeTypeNode, BuildAttributeArguments(generator, message, error, diagnosticId, urlFormat));

        var newMemberNode = generator.AddAttributes(member, obsoleteAttributeNode);
        var newRoot = generator.ReplaceNode(root, member, newMemberNode);
        var newDocument = document.WithSyntaxRoot(newRoot);

        return newDocument;
    }

    static async Task<Document> AddMissingConstructorArguments(Document document, Location location, string message, string error, string? diagnosticId, string? urlFormat, CancellationToken cancellationToken)
    {
        if (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is not { } root ||
            root.FindNode(location.SourceSpan) is not AttributeSyntax original ||
            await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false) is not { } semanticModel ||
            semanticModel.Compilation.GetTypeByMetadataName("System.ObsoleteAttribute") is not { } obsoleteAttributeTypeSymbol)
        {
            return document;
        }

        var generator = SyntaxGenerator.GetGenerator(document);

        var obsoleteAttributeTypeNode = generator.TypeExpression(obsoleteAttributeTypeSymbol).WithAdditionalAnnotations(Simplifier.AddImportsAnnotation);
        var obsoleteAttributeNode = generator.Attribute(obsoleteAttributeTypeNode, BuildAttributeArguments(generator, message, error, diagnosticId, urlFormat));

        var newRoot = generator.ReplaceNode(root, original, obsoleteAttributeNode);
        var newDocument = document.WithSyntaxRoot(newRoot);

        return newDocument;
    }

    static List<SyntaxNode> BuildAttributeArguments(SyntaxGenerator generator, string message, string error, string? diagnosticId, string? urlFormat)
    {
        var arguments = new List<SyntaxNode>
        {
            generator.AttributeArgument(generator.LiteralExpression(message)),
            generator.AttributeArgument(generator.LiteralExpression(bool.Parse(error)))
        };

        if (!string.IsNullOrEmpty(diagnosticId))
        {
            arguments.Add(generator.AttributeArgument("DiagnosticId", generator.LiteralExpression(diagnosticId)));
        }

        if (!string.IsNullOrEmpty(urlFormat))
        {
            arguments.Add(generator.AttributeArgument("UrlFormat", generator.LiteralExpression(urlFormat)));
        }

        return arguments;
    }

    static Task<Document> FixIncorrectObsoleteAttributeMessageArgument(Document document, Location location, string message, CancellationToken cancellationToken) => FixIncorrectObsoleteAttributeArgument(document, location, generator => generator.LiteralExpression(message), cancellationToken);

    static Task<Document> FixIncorrectObsoleteAttributeErrorArgument(Document document, Location location, string error, CancellationToken cancellationToken) => FixIncorrectObsoleteAttributeArgument(document, location, generator => generator.LiteralExpression(bool.Parse(error)), cancellationToken);

    static async Task<Document> FixIncorrectObsoleteAttributeArgument(Document document, Location location, Func<SyntaxGenerator, SyntaxNode> literalExpression, CancellationToken cancellationToken)
    {
        if (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is not { } root || root.FindNode(location.SourceSpan) is not AttributeArgumentSyntax original)
        {
            return document;
        }

        var generator = SyntaxGenerator.GetGenerator(document);

        var newArgument = generator.AttributeArgument(literalExpression(generator));
        var newRoot = generator.ReplaceNode(root, original, newArgument);
        var newDocument = document.WithSyntaxRoot(newRoot);

        return newDocument;
    }

    static Task<Document> FixIncorrectObsoleteAttributeDiagnosticIdArgument(Document document, Location location, string? diagnosticId, CancellationToken cancellationToken)
        => FixIncorrectObsoleteAttributeNamedArgument(document, location, "DiagnosticId", diagnosticId, cancellationToken);

    static Task<Document> FixIncorrectObsoleteAttributeUrlFormatArgument(Document document, Location location, string? urlFormat, CancellationToken cancellationToken)
        => FixIncorrectObsoleteAttributeNamedArgument(document, location, "UrlFormat", urlFormat, cancellationToken);

    static async Task<Document> FixIncorrectObsoleteAttributeNamedArgument(Document document, Location location, string name, string? value, CancellationToken cancellationToken)
    {
        if (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is not { } root)
        {
            return document;
        }

        var node = root.FindNode(location.SourceSpan);
        var generator = SyntaxGenerator.GetGenerator(document);

        // Case 1: location points to the named argument itself (replace value or remove)
        if (node is AttributeArgumentSyntax existingArg && existingArg.NameEquals?.Name.Identifier.ValueText == name)
        {
            if (string.IsNullOrEmpty(value))
            {
                var updatedRoot = generator.RemoveNode(root, existingArg);
                return updatedRoot is not null ? document.WithSyntaxRoot(updatedRoot) : document;
            }

            var newArg = generator.AttributeArgument(name, generator.LiteralExpression(value));
            var newRoot = generator.ReplaceNode(root, existingArg, newArg);
            return document.WithSyntaxRoot(newRoot);
        }

        // Case 2: location points to the Obsolete attribute (add the named argument)
        if (node is AttributeSyntax attr)
        {
            if (string.IsNullOrEmpty(value))
            {
                return document;
            }

            var newArg = generator.AttributeArgument(name, generator.LiteralExpression(value));
            var newAttr = generator.AddAttributeArguments(attr, [newArg]);
            var newRoot = generator.ReplaceNode(root, attr, newAttr);
            return document.WithSyntaxRoot(newRoot);
        }

        return document;
    }
}