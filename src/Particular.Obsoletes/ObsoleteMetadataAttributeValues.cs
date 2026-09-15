namespace Particular.Obsoletes;

readonly record struct ObsoleteMetadataAttributeValues(string? Message, string? TreatAsErrorFromVersion, bool TreatAsErrorFromVersionSet, string? RemoveInVersion, bool RemoveInVersionSet, string? ReplacementTypeOrMember, string? DiagnosticId, bool DiagnosticIdSet, string? UrlFormat, bool UrlFormatSet);
