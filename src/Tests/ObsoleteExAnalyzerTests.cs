namespace Tests;

using System.Threading.Tasks;
using NUnit.Framework;
using Particular.Obsoletes;
using Tests.Helpers;

public class ObsoleteExAnalyzerTests : AnalyzerTestFixture<ObsoleteExAnalyzer>
{
    [Test]
    public Task Test()
    {
        var code = """
            [Obsolete]
            [global::Particular.Obsoletes.ObsoleteEx(Message = "blah")]
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
       [[|ObsoleteEx|]]
       public class Foo
       {

       }
       """;

        return Assert(code, DiagnosticIds.MissingObsoleteAttribute);
    }
}
