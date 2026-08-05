namespace Particular.Obsoletes;

using Microsoft.CodeAnalysis;

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor MissingObsoleteMetadataAttribute = new(
        id: DiagnosticIds.MissingObsoleteMetadataAttribute,
        title: "Obsolete attributes should have a corresponding ObsoleteMetadata attribute",
        messageFormat: "The Obsolete attribute does not have a corresponding ObsoleteMetadata attribute",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingTreatAsErrorFromVersion = new(
        id: DiagnosticIds.MissingTreatAsErrorFromVersion,
        title: "TreatAsErrorFromVersion should be specified",
        messageFormat: "The TreatAsErrorFromVersion argument is required but it has not been specified",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingRemoveInVersion = new(
        id: DiagnosticIds.MissingRemoveInVersion,
        title: "RemoveInVersion should be specified",
        messageFormat: "The RemoveInVersion argument is required but it has not been specified",
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

    public static readonly DiagnosticDescriptor MissingObsoleteAttribute = new(
        id: DiagnosticIds.MissingObsoleteAttribute,
        title: "ObsoleteMetadata attributes should have a corresponding Obsolete attribute",
        messageFormat: "The ObsoleteMetadata attribute does not have a corresponding Obsolete attribute",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ObsoleteAttributeMissingConstructorArguments = new(
        id: DiagnosticIds.ObsoleteAttributeMissingConstructorArguments,
        title: "Obsolete attributes should specify message and error arguments",
        messageFormat: "The Obsolete attribute does not use the overload that provides both message and error arguments",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor IncorrectObsoleteAttributeMessageArgument = new(
        id: DiagnosticIds.IncorrectObsoleteAttributeMessageArgument,
        title: "Obsolete attributes should have a message value that matches the ObsoleteMetadata attribute",
        messageFormat: "The Obsolete attribute's message value does not match the information specified in the ObsoleteMetadata attribute",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor IncorrectObsoleteAttributeErrorArgument = new(
        id: DiagnosticIds.IncorrectObsoleteAttributeErrorArgument,
        title: "Obsolete attributes should have an error value that matches the ObsoleteMetadata attribute",
        messageFormat: "The Obsolete attribute's error value does not match the information specified in the ObsoleteMetadata attribute",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor IncorrectObsoleteAttributeDiagnosticIdArgument = new(
        id: DiagnosticIds.IncorrectObsoleteAttributeDiagnosticIdArgument,
        title: "Obsolete attributes should have a DiagnosticId value that matches the ObsoleteMetadata attribute",
        messageFormat: "The Obsolete attribute's DiagnosticId value does not match the information specified in the ObsoleteMetadata attribute",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor IncorrectObsoleteAttributeUrlFormatArgument = new(
        id: DiagnosticIds.IncorrectObsoleteAttributeUrlFormatArgument,
        title: "Obsolete attributes should have a UrlFormat value that matches the ObsoleteMetadata attribute",
        messageFormat: "The Obsolete attribute's UrlFormat value does not match the information specified in the ObsoleteMetadata attribute",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidDiagnosticId = new(
        id: DiagnosticIds.InvalidDiagnosticId,
        title: "DiagnosticId should not be empty or contain whitespace",
        messageFormat: "The value specified for DiagnosticId '{0}' is empty or contains whitespace, which prevents it from being used with #pragma warning disable or NoWarn",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidUrlFormat = new(
        id: DiagnosticIds.InvalidUrlFormat,
        title: "UrlFormat should contain at most one placeholder",
        messageFormat: "The value specified for UrlFormat '{0}' contains more than one placeholder. The compiler will ignore UrlFormat when multiple placeholders are present.",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor UrlFormatPlaceholderRequiresDiagnosticId = new(
        id: DiagnosticIds.UrlFormatPlaceholderRequiresDiagnosticId,
        title: "UrlFormat with placeholder requires DiagnosticId",
        messageFormat: "The UrlFormat '{0}' contains a placeholder but no DiagnosticId is specified. The compiler will substitute the default 'CS0618', which is unlikely to produce a valid URL.",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}