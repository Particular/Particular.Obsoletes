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

    public static readonly DiagnosticDescriptor MissingRemoveInVersion = new(
        id: DiagnosticIds.MissingRemoveInVersion,
        title: "RemoveInVersion properties should be specified",
        messageFormat: "RemoveInVersion is required but it has not been specified",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingTreatAsErrorFromVersion = new(
        id: DiagnosticIds.MissingTreatAsErrorFromVersion,
        title: "TreatAsErrorFromVersion should be specified",
        messageFormat: "TreatAsErrorFromVersion is required but it has not been specified",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidTreatAsErrorFromVersion = new(
        id: DiagnosticIds.InvalidTreatAsErrorFromVersion,
        title: "TreatAsErrorFromVersion should be a valid Version",
        messageFormat: "The value specified for TreatAsErrorFromVersion '{0}' cannot be parsed as a valid Version",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidRemoveInVersion = new(
        id: DiagnosticIds.InvalidRemoveInVersion,
        title: "RemoveInVersion should be a valid Version",
        messageFormat: "The value specified for RemoveInVersion '{0}' cannot be parsed as a valid Version",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor RemoveInVersionLessThanOrEqualToTreatAsErrorFromVersion = new(
        id: DiagnosticIds.RemoveInVersionLessThanOrEqualToTreatAsErrorFromVersion,
        title: "RemoveInVersion should be greater than TreatAsErrorFromVersion",
        messageFormat: "The version specified in RemoveInVersion '{0}' is less than or equal to the version specified in TreatAsErrorFromVersion '{1}'. RemoveInVersion should be increased.",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor RemoveObsoleteMember = new(
        id: DiagnosticIds.RemoveObsoleteMember,
        title: "Obsolete members should be removed",
        messageFormat: "The assembly version '{0}' is greater than or equal to the version specified in RemoveInVersion '{1}'. The member should be removed or RemoveInVersion increased.",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ObsoleteAttributeMissingConstructorArguments = new(
        id: DiagnosticIds.ObsoleteAttributeMissingConstructorArguments,
        title: "Obsolete attributes should have two constructor arguments",
        messageFormat: "Obsolete attribute should have two constructor arguments but has {0}",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor IncorrectObsoleteAttributeMessageArgument = new(
        id: DiagnosticIds.IncorrectObsoleteAttributeMessageArgument,
        title: "Obsolete attributes should have a message that matches the ObsoleteMetadata attribute",
        messageFormat: "Fix the Obsolete message",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor IncorrectObsoleteAttributeIsErrorArgument = new(
        id: DiagnosticIds.IncorrectObsoleteAttributeIsErrorArgument,
        title: "Obsolete attributes should have an isError that matches the ObsoleteMetadata attribute",
        messageFormat: "Fix the Obsolete isError",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
