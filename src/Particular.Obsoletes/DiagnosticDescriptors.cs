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

    public static readonly DiagnosticDescriptor RemoveObsoleteMember = new(
        id: DiagnosticIds.RemoveObsoleteMember,
        title: "Obsolete members should be removed",
        messageFormat: "The assembly version {0} is equal to or greater than the version specified in 'RemoveInVersion' {1}. The member should be removed or 'RemoveInVersion' increased.",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
