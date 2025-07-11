namespace Tests;

using NUnit.Framework;
using Particular.Obsoletes;
using Particular.Obsoletes.Fixes;
using Tests.Helpers;

public class ObsoleteCodeFixProviderTests : CodeFixTestFixture<ObsoleteAnalyzer, ObsoleteCodeFixProvider>
{
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
}
