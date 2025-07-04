namespace Tests.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

public partial class AnalyzerTestFixture<TAnalyzer> where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected Task Assert(string markupCode, CancellationToken cancellationToken = default) =>
        Assert(markupCode, [], [], cancellationToken);

    protected Task Assert(string markupCode, string expectedDiagnosticId, CancellationToken cancellationToken = default) =>
        Assert(markupCode, [expectedDiagnosticId], [], cancellationToken);

    protected Task Assert(string markupCode, string[] expectedDiagnosticIds, CancellationToken cancellationToken = default) =>
        Assert(markupCode, expectedDiagnosticIds, [], cancellationToken);

    protected async Task Assert(string markupCode, string[] expectedDiagnosticIds, string[] ignoreDiagnosticIds, CancellationToken cancellationToken = default)
    {
        markupCode = AddUsings(markupCode);

        var (code, markupSpans) = Parse(markupCode);

        var project = CreateProject(code);
        await WriteCode(project, cancellationToken);

        var compilerDiagnostics = (await Task.WhenAll(project.Documents
            .Select(doc => doc.GetCompilerDiagnostics(cancellationToken))))
            .SelectMany(diagnostics => diagnostics);

        WriteCompilerDiagnostics(compilerDiagnostics);

        var compilation = await project.GetCompilationAsync(cancellationToken);

        if (compilation is null)
        {
            NUnit.Framework.Assert.That(compilation, Is.Not.Null);
            return;
        }

        compilation.Compile();

        var analyzerDiagnostics = (await compilation.GetAnalyzerDiagnostics(new TAnalyzer(), cancellationToken))
            .Where(d => !ignoreDiagnosticIds.Contains(d.Id))
            .ToList();
        WriteAnalyzerDiagnostics(analyzerDiagnostics);

        var expectedSpansAndIds = expectedDiagnosticIds
            .SelectMany(id => markupSpans.Select(span => (span.file, span.span, id)))
            .OrderBy(item => item.span)
            .ThenBy(item => item.id)
            .ToList();

        var actualSpansAndIds = analyzerDiagnostics
            .Select(diagnostic => (diagnostic.Location.SourceTree?.FilePath, diagnostic.Location.SourceSpan, diagnostic.Id))
            .ToList();

        NUnit.Framework.Assert.That(actualSpansAndIds, Is.EqualTo(expectedSpansAndIds).AsCollection);
    }

    protected static string AddUsings(string markupCode) =>
        """
        using System;
        using System.Threading;
        using System.Threading.Tasks;
        using Particular.Obsoletes;


        """ + markupCode;

    protected static async Task WriteCode(Project project, CancellationToken cancellationToken = default)
    {
        foreach (var document in project.Documents)
        {
            TestContext.Out.WriteLine(document.Name);
            var code = await document.GetCode(cancellationToken);
            foreach (var (line, index) in code.Replace("\r\n", "\n").Split('\n')
            .Select((line, index) => (line, index)))
            {
                TestContext.Out.WriteLine($"  {index + 1,3}: {line}");
            }
        }
    }

    protected Project CreateProject(string[] code)
    {
        var netStandard = AssemblyLoadContext.Default.Assemblies.First(a => a.FullName?.StartsWith("netstandard") ?? false);
        var systemRuntime = AssemblyLoadContext.Default.Assemblies.First(a => a.FullName?.StartsWith("System.Runtime") ?? false);

        var project = new AdhocWorkspace()
            .AddProject("TestProject", LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddMetadataReference(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
            .AddMetadataReference(MetadataReference.CreateFromFile(netStandard.Location))
            .AddMetadataReference(MetadataReference.CreateFromFile(systemRuntime.Location))
            .AddMetadataReference(MetadataReference.CreateFromFile("Particular.Obsoletes.Attributes.dll"));

        for (int i = 0; i < code.Length; i++)
        {
            project = project.AddDocument($"TestDocument{i}", code[i]).Project;
        }

        return project;
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

    protected static string[] SplitMarkupCodeIntoFiles(string markupCode) => [.. DocumentSplittingRegex().Split(markupCode).Where(docCode => !string.IsNullOrWhiteSpace(docCode))];

    static (string[] code, List<(string file, TextSpan span)>) Parse(string markupCode)
    {
        if (markupCode is null)
        {
            return ([], []);
        }

        var documents = SplitMarkupCodeIntoFiles(markupCode);

        var markupSpans = new List<(string, TextSpan)>();

        for (var i = 0; i < documents.Length; i++)
        {
            var code = new StringBuilder();
            var name = $"TestDocument{i}";

            var remainingCode = documents[i];
            var remainingCodeStart = 0;

            while (remainingCode.Length > 0)
            {
                var beforeAndAfterOpening = remainingCode.Split(openingSeparator, 2, StringSplitOptions.None);

                if (beforeAndAfterOpening.Length == 1)
                {
                    _ = code.Append(beforeAndAfterOpening[0]);
                    break;
                }

                var midAndAfterClosing = beforeAndAfterOpening[1].Split(closingSeparator, 2, StringSplitOptions.None);

                if (midAndAfterClosing.Length == 1)
                {
                    throw new Exception("The markup code does not contain a closing '|]'");
                }

                var markupSpan = new TextSpan(remainingCodeStart + beforeAndAfterOpening[0].Length, midAndAfterClosing[0].Length);

                _ = code.Append(beforeAndAfterOpening[0]).Append(midAndAfterClosing[0]);
                markupSpans.Add((name, markupSpan));

                remainingCode = midAndAfterClosing[1];
                remainingCodeStart += beforeAndAfterOpening[0].Length + markupSpan.Length;
            }

            documents[i] = code.ToString();
        }

        return (documents, markupSpans);
    }

    static readonly string[] openingSeparator = ["[|"];
    static readonly string[] closingSeparator = ["|]"];

    [GeneratedRegex("^-{5,}.*", RegexOptions.Multiline)]
    private static partial Regex DocumentSplittingRegex();
}
