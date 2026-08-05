namespace Tests;

using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Particular.AnalyzerTesting;
using Particular.Obsoletes;
using Particular.Obsoletes.Fixes;

public class ObsoleteCodeFixProviderTests : CodeFixTestFixture<ObsoleteAnalyzer, ObsoleteCodeFixProvider>
{
    static readonly MetadataReference Obsoletes = MetadataReference.CreateFromFile(typeof(ObsoleteMetadataAttribute).Assembly.Location);

    protected override void ConfigureFixtureTests(CodeFixTest test) => test.AddReferences(Obsoletes);

    [Test]
    public Task MissingObsolete()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("2.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("2.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be removed in version 3.0.0.", true)]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task MissingObsolete_NeedsUsing_UsingsInside()
    {
        var original = """
        namespace Blah;

        using Particular.Obsoletes;

        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        public class Foo
        {

        }
        """;

        var expected = """
        namespace Blah;

        using System;
        using Particular.Obsoletes;

        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false)]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task MissingObsolete_NeedsUsing_UsingsOutside()
    {
        var original = """
        using Particular.Obsoletes;

        namespace Blah;

        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        namespace Blah;

        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false)]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task MissingConstructorArguments_None()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false)]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task MissingConstructorArguments_Error()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.")]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false)]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task IncorrectObsoleteAttributeMessageArgument()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("", false)]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false)]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task IncorrectObsoleteAttributeErrorArgument()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", true)]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false)]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task MissingObsolete_WithDiagnosticIdAndTemplateUrlFormat()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("2.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", DiagnosticId = "NSB0001", UrlFormat = "https://docs.particular.net/r/obsoletions/{0}")]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("2.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", DiagnosticId = "NSB0001", UrlFormat = "https://docs.particular.net/r/obsoletions/{0}")]
        [Obsolete("Will be removed in version 3.0.0.", true, DiagnosticId = "NSB0001", UrlFormat = "https://docs.particular.net/r/obsoletions/{0}")]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task MissingObsolete_WithLiteralUrlFormatWithoutDiagnosticId()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("2.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", UrlFormat = "https://github.com/Particular/NServiceBus/issues/42")]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("2.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", UrlFormat = "https://github.com/Particular/NServiceBus/issues/42")]
        [Obsolete("Will be removed in version 3.0.0.", true, UrlFormat = "https://github.com/Particular/NServiceBus/issues/42")]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task MissingConstructorArguments_WithDiagnosticIdAndTemplateUrlFormat()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", DiagnosticId = "NSB0001", UrlFormat = "https://docs.particular.net/r/obsoletions/{0}")]
        [Obsolete]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", DiagnosticId = "NSB0001", UrlFormat = "https://docs.particular.net/r/obsoletions/{0}")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false, DiagnosticId = "NSB0001", UrlFormat = "https://docs.particular.net/r/obsoletions/{0}")]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task IncorrectObsoleteAttributeDiagnosticIdArgument_WrongValue()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", DiagnosticId = "NSB0001")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false, DiagnosticId = "WRONG")]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", DiagnosticId = "NSB0001")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false, DiagnosticId = "NSB0001")]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task IncorrectObsoleteAttributeDiagnosticIdArgument_Missing()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", DiagnosticId = "NSB0001")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false)]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", DiagnosticId = "NSB0001")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false, DiagnosticId = "NSB0001")]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task IncorrectObsoleteAttributeDiagnosticIdArgument_ExtraOnObsolete()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false, DiagnosticId = "NSB0001")]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false)]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task IncorrectObsoleteAttributeUrlFormatArgument_WrongValue()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", DiagnosticId = "NSB0001", UrlFormat = "https://docs.particular.net/obsoletions/{0}")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false, DiagnosticId = "NSB0001", UrlFormat = "https://wrong.com/{0}")]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", DiagnosticId = "NSB0001", UrlFormat = "https://docs.particular.net/r/obsoletions/{0}")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false, DiagnosticId = "NSB0001", UrlFormat = "https://docs.particular.net/r/obsoletions/{0}")]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task IncorrectObsoleteAttributeUrlFormatArgument_Missing()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", UrlFormat = "https://github.com/Particular/NServiceBus/issues/42")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false)]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", UrlFormat = "https://github.com/Particular/NServiceBus/issues/42")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false, UrlFormat = "https://github.com/Particular/NServiceBus/issues/42")]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task IncorrectObsoleteAttributeUrlFormatArgument_ExtraOnObsolete()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false, UrlFormat = "https://github.com/Particular/NServiceBus/issues/42")]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false)]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task IncorrectObsoleteAttributeDiagnosticIdAndUrlFormatArguments_BothWrong()
    {
        var original = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", DiagnosticId = "NSB0001", UrlFormat = "https://docs.particular.net/r/obsoletions/{0}")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false, DiagnosticId = "WRONG", UrlFormat = "https://wrong.com/{0}")]
        public class Foo
        {

        }
        """;

        var expected = """
        using System;
        using Particular.Obsoletes;

        [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3", DiagnosticId = "NSB0001", UrlFormat = "https://docs.particular.net/r/obsoletions/{0}")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false, DiagnosticId = "NSB0001", UrlFormat = "https://docs.particular.net/r/obsoletions/{0}")]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }
}