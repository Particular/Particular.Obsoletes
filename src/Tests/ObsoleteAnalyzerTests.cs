namespace Tests;

using System.Threading.Tasks;
using NUnit.Framework;
using Particular.Obsoletes;
using Tests.Helpers;

public class ObsoleteAnalyzerTests : AnalyzerTestFixture<ObsoleteAnalyzer>
{
    [Test]
    public Task Test()
    {
        var code = """
            [Obsolete]
            [global::Particular.Obsoletes.ObsoleteMetadata(Message = "blah")]
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
    public Task MissingObsolete()
    {
        var code = """
       [[|ObsoleteMetadata|]]
       public class Foo
       {

       }
       """;

        return Assert(code, DiagnosticIds.MissingObsoleteAttribute);
    }

    [Test]
    public Task RemoveObsoleteMember()
    {
        var code = """
        [assembly: System.Reflection.AssemblyVersionAttribute("3.0.0.0")]

        [[|ObsoleteMetadata(TreatAsErrorFromVersion = "2.0", RemoveInVersion = "3.0")|]]
        [Obsolete]
        public class Foo
        {

        }
        """;

        return Assert(code, DiagnosticIds.RemoveObsoleteMember);
    }
}
