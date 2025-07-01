namespace Particular.Obsoletes;

#if FIXES
static class DiagnosticIds
#else
public static class DiagnosticIds
#endif
{
    public const string MissingObsoleteMetadataAttribute = "OBSOLETES0001";
    public const string MissingObsoleteAttribute = "OBSOLETES0002";
}
