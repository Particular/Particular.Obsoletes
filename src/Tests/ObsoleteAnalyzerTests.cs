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
}
