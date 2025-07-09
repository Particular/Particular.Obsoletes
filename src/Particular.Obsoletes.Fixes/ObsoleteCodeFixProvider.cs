namespace Particular.Obsoletes.Fixes;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

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
        var diagnostic = context.Diagnostics.First();

        diagnostic.Properties.TryGetValue("Message", out var message);
        diagnostic.Properties.TryGetValue("IsError", out var isError);

        var codeAction = CodeAction.Create("Add missing Obsolete attribute", token => AddMissingObsolete(context.Document, diagnostic.Location, message ?? string.Empty, isError ?? string.Empty, token), "Add missing Obsolete attribute");

        context.RegisterCodeFix(codeAction, diagnostic);

        return Task.CompletedTask;
    }

    static async Task<Document> AddMissingObsolete(Document document, Location location, string message, string isError, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        if (root is null)
        {
            return document;
        }

        var obsoleteMetadataAttributeNode = root.FindNode(location.SourceSpan);

        if (obsoleteMetadataAttributeNode.Parent?.Parent is not MemberDeclarationSyntax member)
        {
            return document;
        }

        var generator = SyntaxGenerator.GetGenerator(document);

        var obsoleteAttributeNode = generator.Attribute("Obsolete", generator.AttributeArgument(generator.LiteralExpression(message)), generator.AttributeArgument(generator.LiteralExpression(bool.Parse(isError))));

        var newNode = generator.AddAttributes(member, obsoleteAttributeNode);
        var newRoot = generator.ReplaceNode(root, member, newNode);


        bool systemUsingDirectiveNeeded = true;
        SyntaxNode? foo = member.Parent;

        do
        {
            SyntaxList<UsingDirectiveSyntax>? usings = null;

            if (foo is NamespaceDeclarationSyntax namespaceDeclaration)
            {
                usings = namespaceDeclaration.Usings;
            }
            else if (foo is FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDeclaration)
            {
                usings = fileScopedNamespaceDeclaration.Usings;
            }
            else if (foo is CompilationUnitSyntax compilationUnit)
            {
                usings = compilationUnit.Usings;
            }

            if (usings is not null)
            {
                foreach (var usingDirective in usings)
                {
                    if (usingDirective is UsingDirectiveSyntax { Name: IdentifierNameSyntax { Identifier.Text: nameof(System) } })
                    {
                        systemUsingDirectiveNeeded = false;
                        break;
                    }
                }
            }

            foo = foo?.Parent;

        } while (systemUsingDirectiveNeeded && foo is not null);


        if (systemUsingDirectiveNeeded)
        {
            newRoot = generator.AddNamespaceImports(newRoot, generator.NamespaceImportDeclaration(nameof(System)));
        }

        var newDocument = document.WithSyntaxRoot(newRoot);

        return newDocument;
    }

}
