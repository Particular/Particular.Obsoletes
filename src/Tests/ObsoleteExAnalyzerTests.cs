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

            using System.Diagnostics;
            using Particular.Obsoletes;

            namespace Particular.Obsoletes
            {
            [Conditional("PARTICULAR_OBSOLETES")]
            [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
            sealed class ObsoleteExAttribute : Attribute
            {
                public string Message { get; set; }

                public string TreatAsErrorFromVersion { get; set; }

                public string RemoveInVersion { get; set; }

               public string ReplacementTypeOrMember { get; set; }
            }
            }



            [Obsolete]
            [global::Particular.Obsoletes.ObsoleteEx(Message = "blah")]
            public class Foo
            {

            }
            """;

        return Assert(code);
    }
}
