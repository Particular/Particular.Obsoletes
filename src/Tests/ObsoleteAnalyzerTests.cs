namespace Tests;

using System.Threading.Tasks;
using NUnit.Framework;
using Particular.Obsoletes;
using Tests.Helpers;

public class ObsoleteAnalyzerTests : AnalyzerTestFixture<ObsoleteAnalyzer>
{
    [Test]
    public Task NoErrors()
    {
        var code = """
            [assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]

            [ObsoleteMetadata(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "3.0")]
            [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", false)]
            public class Foo
            {

            }
            """;

        return Assert(code);
    }

    [Test]
    public Task MissingObsoleteMetadata()
    {
        var code = """
         [[|Obsolete|]]
         public class Foo
         {

         }
         """;

        return Assert(code, DiagnosticIds.MissingObsoleteMetadataAttribute);
    }

    [Test]
    public Task MissingTreatAsErrorFromVersion()
    {
        var code = """
     [[|ObsoleteMetadata(RemoveInVersion = "3")|]]
     public class Foo
     {

     }
     """;

        return Assert(code, DiagnosticIds.MissingTreatAsErrorFromVersion);
    }

    [Test]
    public Task MissingRemoveInVersion()
    {
        var code = """
     [[|ObsoleteMetadata(TreatAsErrorFromVersion = "2")|]]
     public class Foo
     {

     }
     """;

        return Assert(code, DiagnosticIds.MissingRemoveInVersion);
    }

    [Test]
    public Task InvalidTreatAsErrorFromVersion()
    {
        var code = """
        [ObsoleteMetadata([|TreatAsErrorFromVersion = "not-a-version"|], RemoveInVersion = "3")]
        public class Foo
        {

        }
        """;

        return Assert(code, DiagnosticIds.InvalidTreatAsErrorFromVersion);
    }

    [Test]
    public Task InvalidRemoveInVersion()
    {
        var code = """
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", [|RemoveInVersion = "not-a-version"|])]
        public class Foo
        {

        }
        """;

        return Assert(code, DiagnosticIds.InvalidRemoveInVersion);
    }

    [Test]
    public Task RemoveInVersionLessThanOrEqualToTreatAsErrorFromVersion()
    {
        var code = """
        [[|ObsoleteMetadata(TreatAsErrorFromVersion = "3", RemoveInVersion = "2")|]]
        public class Foo
        {

        }
        """;

        return Assert(code, DiagnosticIds.RemoveInVersionLessThanOrEqualToTreatAsErrorFromVersion);
    }

    [Test]
    public Task RemoveObsoleteMember()
    {
        var code = """
        [assembly: System.Reflection.AssemblyVersionAttribute("3.0.0.0")]

        [|[ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        public class Foo
        {

        }|]
        """;

        return Assert(code, DiagnosticIds.RemoveObsoleteMember);
    }

    [Test]
    public Task MissingObsolete()
    {
        var code = """
       [[|ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")|]]
       public class Foo
       {

       }
       """;

        return Assert(code, DiagnosticIds.MissingObsoleteAttribute);
    }

    [Test]
    public Task ObsoleteAttributeMissingConstructorArguments()
    {
        var code = """
       [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
       [[|Obsolete|]]
       public class Foo
       {

       }
       """;

        return Assert(code, DiagnosticIds.ObsoleteAttributeMissingConstructorArguments);
    }

    [Test]
    public Task IncorrectObsoleteAttributeMessageArgument()
    {
        var code = """
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete([|""|], false)]
        public class Foo
        {

        }
        """;

        return Assert(code, DiagnosticIds.IncorrectObsoleteAttributeMessageArgument);
    }

    [Test]
    public Task IncorrectObsoleteAttributeIsErrorArgument()
    {
        var code = """
        [ObsoleteMetadata(TreatAsErrorFromVersion = "2", RemoveInVersion = "3")]
        [Obsolete("Will be treated as an error from version 2.0.0. Will be removed in version 3.0.0.", [|true|])]
        public class Foo
        {

        }
        """;

        return Assert(code, DiagnosticIds.IncorrectObsoleteAttributeIsErrorArgument);
    }
}
