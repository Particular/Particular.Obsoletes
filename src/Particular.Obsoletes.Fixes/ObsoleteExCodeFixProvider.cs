namespace Particular.Obsoletes.Fixes;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeFixes;

class ObsoleteExCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => throw new NotImplementedException();

    public override Task RegisterCodeFixesAsync(CodeFixContext context) => throw new NotImplementedException();
}
