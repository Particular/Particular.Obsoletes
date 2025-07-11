namespace Particular.Obsoletes;

readonly record struct ObsoleteMetadataAttributeValues
{
    public ObsoleteMetadataAttributeValues(string? message, string? treatAsErrorFromVersion, bool treatAsErrorFromVersionSet, string? removeInVersion, bool removeInVersionSet, string? replacementTypeOrMember)
    {
        Message = message;

        TreatAsErrorFromVersion = treatAsErrorFromVersion;
        TreatAsErrorFromVersionSet = treatAsErrorFromVersionSet;

        RemoveInVersion = removeInVersion;
        RemoveInVersionSet = removeInVersionSet;

        ReplacementTypeOrMember = replacementTypeOrMember;
    }

    public readonly string? Message;
    public readonly string? TreatAsErrorFromVersion;
    public readonly bool TreatAsErrorFromVersionSet;
    public readonly string? RemoveInVersion;
    public readonly bool RemoveInVersionSet;
    public readonly string? ReplacementTypeOrMember;
}
