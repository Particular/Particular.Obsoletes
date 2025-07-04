namespace Particular.Obsoletes.Fixes;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;

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
        var properties = diagnostic.Properties;
        _ = properties;

        return Task.CompletedTask;
    }

}
