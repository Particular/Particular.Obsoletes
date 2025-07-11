namespace Particular.Obsoletes;

#if FIXES
static class DiagnosticIds
#else
public static class DiagnosticIds
#endif
{
    const string Prefix = "OBSOLETES";

    public const string MissingObsoleteMetadataAttribute = Prefix + "0001";
    public const string MissingTreatAsErrorFromVersion = Prefix + "0002";
    public const string MissingRemoveInVersion = Prefix + "0003";
    public const string InvalidTreatAsErrorFromVersion = Prefix + "0004";
    public const string InvalidRemoveInVersion = Prefix + "0005";
    public const string RemoveInVersionLessThanOrEqualToTreatAsErrorFromVersion = Prefix + "0006";
    public const string RemoveObsoleteMember = Prefix + "0007";
    public const string MissingObsoleteAttribute = Prefix + "0008";
    public const string ObsoleteAttributeMissingConstructorArguments = Prefix + "0009";
    public const string IncorrectObsoleteAttributeMessageArgument = Prefix + "0010";
    public const string IncorrectObsoleteAttributeErrorArgument = Prefix + "0011";
}
