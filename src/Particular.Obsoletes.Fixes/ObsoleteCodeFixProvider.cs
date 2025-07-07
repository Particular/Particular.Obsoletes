namespace Particular.Obsoletes.Fixes;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[ExportCodeFixProvider(LanguageNames.CSharp)]
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
        var diagnostic = context.Diagnostics.First();

        diagnostic.Properties.TryGetValue("Message", out var message);
        diagnostic.Properties.TryGetValue("IsError", out var isError);

        var codeAction = CodeAction.Create("", token => AddMissingObsolete(context.Document, diagnostic.Location, message, isError, token), "");

        context.RegisterCodeFix(codeAction, diagnostic);

        return Task.CompletedTask;
    }

    static async Task<Document> AddMissingObsolete(Document document, Location location, string? message, string? isError, CancellationToken cancellationToken)
    {
        var name = SyntaxFactory.IdentifierName("Obsolete");

        var messageExpression = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(message));
        var messageArgument = SyntaxFactory.AttributeArgument(messageExpression);

        var isErrorLiteral = isError switch
        {
            "False" => SyntaxKind.FalseLiteralExpression,
            "True" => SyntaxKind.TrueLiteralExpression,
            _ => SyntaxKind.FalseLiteralExpression
        };

        var isErrorExpression = SyntaxFactory.LiteralExpression(isErrorLiteral);
        var isErrorArgument = SyntaxFactory.AttributeArgument(isErrorExpression);

        var attributeArgumentList = SyntaxFactory.AttributeArgumentList([messageArgument, isErrorArgument]);

        var obsoleteAttribute = SyntaxFactory.Attribute(name, attributeArgumentList);

        var attributeList = SyntaxFactory.AttributeList([obsoleteAttribute]);

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        var node = root.FindNode(location.SourceSpan);
        var member = node.Parent.Parent as MemberDeclarationSyntax;
        var result = member.AddAttributeLists(attributeList);

        var newRoot = root.ReplaceNode(member, result);

        var newDocument = document.WithSyntaxRoot(newRoot);

        _ = isError;

        return newDocument;
    }

}
