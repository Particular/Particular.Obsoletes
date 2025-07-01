namespace Particular.Obsoletes.Fixes;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeFixes;

class ObsoleteCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => throw new NotImplementedException();

    public override Task RegisterCodeFixesAsync(CodeFixContext context) => throw new NotImplementedException();
}
