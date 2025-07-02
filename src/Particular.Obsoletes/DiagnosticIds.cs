namespace Particular.Obsoletes;

#if FIXES
static class DiagnosticIds
#else
public static class DiagnosticIds
#endif
{
    public const string MissingObsoleteMetadataAttribute = "OBSOLETES0001";
    public const string MissingObsoleteAttribute = "OBSOLETES0002";
    public const string InvalidTreatAsErrorFromVersion = "OBSOLETES0003";
    public const string RemoveInVersionLessThanOrEqualToTreatAsErrorFromVersion = "OBSOLETES0004";
    public const string RemoveObsoleteMember = "OBSOLETES0005";
    public const string ObsoleteAttributeMissingConstructorArguments = "OBSOLETES0006";
    public const string IncorrectObsoleteAttributeMessageArgument = "OBSOLETES0007";
    public const string IncorrectObsoleteAttributeIsErrorArgument = "OBSOLETES0008";
    public const string MissingRemoveInVersion = "OBSOLETES0009";
    public const string MissingTreatAsErrorFromVersion = "OBSOLETES0010";
    public const string InvalidRemoveInVersion = "OBSOLETES0011";
}
