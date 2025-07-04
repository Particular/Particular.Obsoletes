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
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        public class Foo
        {

        }
        """;

        var expected = """
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false)]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }

    [Test]
    public Task BothArgumentsIncorrect()
    {
        var original = """
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("", true)]
        public class Foo
        {

        }
        """;

        var expected = """
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false)]
        public class Foo
        {

        }
        """;

        return Assert(original, expected);
    }
}
