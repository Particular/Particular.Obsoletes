namespace Tests.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

public class AnalyzerTestFixture<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected static readonly List<string> PrivateModifiers = ["", "private"];

    protected static readonly List<string> NonPrivateModifiers = ["public", "protected", "internal", "protected internal", "private protected"];

    protected static readonly List<string> InterfacePrivateModifiers =
    [
#if NET
        "private",
#endif
    ];

    protected static readonly List<string> InterfaceNonPrivateModifiers =
    [
        "",
        "public",
        "internal",
#if NET
        "protected",
        "protected internal",
        "private protected",
#endif
    ];

    protected Task Assert(string markupCode, Action<TestCustomizations>? customize = null, CancellationToken cancellationToken = default) =>
        Assert(markupCode, [], customize, cancellationToken);

    protected Task Assert(string markupCode, string expectedDiagnosticId, Action<TestCustomizations>? customize = null, CancellationToken cancellationToken = default) =>
        Assert(markupCode, [expectedDiagnosticId], customize, cancellationToken);

    protected async Task Assert(string markupCode, string[] expectedDiagnosticIds, Action<TestCustomizations>? customize = null, CancellationToken cancellationToken = default)
    {
        var testCustomizations = new TestCustomizations();
        customize?.Invoke(testCustomizations);

        markupCode = """
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using Particular.Obsoletes;


        """ + markupCode;

        var (code, markupSpans) = Parse(markupCode);
        WriteCode(code);

        var document = CreateDocument(code, testCustomizations);

        var compilerDiagnostics = await document.GetCompilerDiagnostics(cancellationToken);
        WriteCompilerDiagnostics(compilerDiagnostics);

        var compilation = await document.Project.GetCompilationAsync(cancellationToken);

        if (compilation is null)
        {
            NUnit.Framework.Assert.That(compilation, Is.Not.Null);
            return;
        }

        compilation.Compile();

        var analyzerDiagnostics = (await compilation.GetAnalyzerDiagnostics(new TAnalyzer(), cancellationToken)).ToList();
        WriteAnalyzerDiagnostics(analyzerDiagnostics);

        var expectedSpansAndIds = expectedDiagnosticIds
            .SelectMany(id => markupSpans.Select(span => (span, id)))
            .OrderBy(item => item.span)
            .ThenBy(item => item.id)
            .ToList();

        var actualSpansAndIds = analyzerDiagnostics
            .Select(diagnostic => (diagnostic.Location.SourceSpan, diagnostic.Id))
            .ToList();

        NUnit.Framework.Assert.That(actualSpansAndIds, Is.EqualTo(expectedSpansAndIds));
    }

    protected static void WriteCode(string? code)
    {
        if (code is null)
        {
            return;
        }

        foreach (var (line, index) in code.Replace("\r\n", "\n").Split('\n')
            .Select((line, index) => (line, index)))
        {
            TestContext.Out.WriteLine($"  {index + 1,3}: {line}");
        }
    }

    protected static Document CreateDocument(string? code, TestCustomizations customizations)
    {
        var obsoleteMetadata = File.ReadAllText("ObsoleteMetadataAttribute.cs");

        return new AdhocWorkspace()
            .AddProject("TestProject", LanguageNames.CSharp)
            .WithCompilationOptions(customizations.CompilationOptions ?? new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddMetadataReferences(customizations.GetMetadataReferences())
            .AddDocument("ObsoleteMetadata", SourceText.From(obsoleteMetadata, Encoding.UTF8))
            .Project
            .AddDocument("TestDocument", code ?? string.Empty);
    }

    protected static void WriteCompilerDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        TestContext.Out.WriteLine("Compiler diagnostics:");

        foreach (var diagnostic in diagnostics)
        {
            TestContext.Out.WriteLine($"  {diagnostic}");
        }
    }

    protected static void WriteAnalyzerDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        TestContext.Out.WriteLine("Analyzer diagnostics:");

        foreach (var diagnostic in diagnostics)
        {
            TestContext.Out.WriteLine($"  {diagnostic}");
        }
    }

    static (string?, List<TextSpan>) Parse(string? markupCode)
    {
        if (markupCode is null)
        {
            return (null, []);
        }

        var code = new StringBuilder();
        var markupSpans = new List<TextSpan>();

        var remainingCode = markupCode;
        var remainingCodeStart = 0;

        while (remainingCode.Length > 0)
        {
            var beforeAndAfterOpening = remainingCode.Split(["[|"], 2, StringSplitOptions.None);

            if (beforeAndAfterOpening.Length == 1)
            {
                _ = code.Append(beforeAndAfterOpening[0]);
                break;
            }

            var midAndAfterClosing = beforeAndAfterOpening[1].Split(["|]"], 2, StringSplitOptions.None);

            if (midAndAfterClosing.Length == 1)
            {
                throw new Exception("The markup code does not contain a closing '|]'");
            }

            var markupSpan = new TextSpan(remainingCodeStart + beforeAndAfterOpening[0].Length, midAndAfterClosing[0].Length);

            _ = code.Append(beforeAndAfterOpening[0]).Append(midAndAfterClosing[0]);
            markupSpans.Add(markupSpan);

            remainingCode = midAndAfterClosing[1];
            remainingCodeStart += beforeAndAfterOpening[0].Length + markupSpan.Length;
        }

        return (code.ToString(), markupSpans);
    }
}
