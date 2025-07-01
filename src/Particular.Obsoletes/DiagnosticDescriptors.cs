namespace Particular.Obsoletes;

using Microsoft.CodeAnalysis;

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor MissingObsoleteMetadataAttribute = new(
        id: DiagnosticIds.MissingObsoleteMetadataAttribute,
        title: "Obsolete attributes should have a corresponding ObsoleteMetadata attribute",
        messageFormat: "Add an ObsoleteMetadata attribute",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingObsoleteAttribute = new(
        id: DiagnosticIds.MissingObsoleteAttribute,
        title: "ObsoleteMetadata attributes should have a corresponding Obsolete attribute",
        messageFormat: "Add an Obsolete attribute",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
