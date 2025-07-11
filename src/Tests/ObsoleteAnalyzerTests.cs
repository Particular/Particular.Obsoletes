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
    public Task MissingObsoleteMetadata2()
    {
        var code = """

         public class Foo
         {
            [[|Obsolete|]]
            public void Method() {}
         }
         """;

        return Assert(code, DiagnosticIds.MissingObsoleteMetadataAttribute);
    }

    [Test]
    public Task MissingObsoleteMetadata3()
    {
        var code = """

         public class Foo
         {
            [[|Obsolete|]]
            public int Property { get; set; }
         }
         """;

        return Assert(code, DiagnosticIds.MissingObsoleteMetadataAttribute);
    }


    [Test]
    public Task MissingObsoleteMetadata4()
    {
        var code = """

         public class Foo
         {
            [[|Obsolete|]]
            public int Field = 42;
         }
         """;

        return Assert(code, DiagnosticIds.MissingObsoleteMetadataAttribute);
    }

    [Test]
    public Task MissingObsoleteMetadata5()
    {
        var code = """

         public class Foo
         {
            [[|Obsolete|]]
            public delegate void Delegate(int Value);
         }
         """;

        return Assert(code, DiagnosticIds.MissingObsoleteMetadataAttribute);
    }

    [Test]
    public Task MissingObsoleteMetadata6()
    {
        var code = """

         public class Foo
         {
            [[|Obsolete|]]
            public event EventHandler<EventArgs> Event;
         }
         """;

        return Assert(code, DiagnosticIds.MissingObsoleteMetadataAttribute);
    }

    [Test]
    public Task MissingObsoleteMetadata7()
    {
        var code = """

         public class Foo
         {
            [[|Obsolete|]]
            public Foo() { }
         }
         """;

        return Assert(code, DiagnosticIds.MissingObsoleteMetadataAttribute);
    }

    [Test]
    public Task MissingObsoleteMetadata8()
    {
        var code = """
         [[|Obsolete|]]
         public struct Foo
         {

         }
         """;

        return Assert(code, DiagnosticIds.MissingObsoleteMetadataAttribute);
    }

    [Test]
    public Task MissingObsoleteMetadata9()
    {
        var code = """
         [[|Obsolete|]]
         public interface Foo
         {

         }
         """;

        return Assert(code, DiagnosticIds.MissingObsoleteMetadataAttribute);
    }

    [Test]
    public Task MissingObsoleteMetadata10()
    {
        var code = """
         [[|Obsolete|]]
         public enum Foo
         {

         }
         """;

        return Assert(code, DiagnosticIds.MissingObsoleteMetadataAttribute);
    }

    [Test]
    public Task MissingObsoleteMetadata11()
    {
        var code = """
         public class Foo
         {
            [[|Obsolete|]]
         	public event EventHandler<EventArgs> Event
         	{
         		add { eventHandler += value; }
         		remove { eventHandler -= value; }
         	}

         	EventHandler<EventArgs> eventHandler;
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

        return Assert(code, DiagnosticIds.IncorrectObsoleteAttributeErrorArgument);
    }
}
