namespace Particular.Obsoletes;

using System;
using System.Diagnostics;

/// <summary>
/// Data that the Particular.Obsoletes analyzer uses to ensure the corresponding <see cref="ObsoleteAttribute" /> is properly constructed.
/// </summary>
[Conditional("PARTICULAR_OBSOLETES")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
public sealed class ObsoleteMetadataAttribute : Attribute
{
    /// <summary>
    /// The text string that describes alternative workarounds.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    ///  A value pointing to the name of the replacement member if available.
    /// </summary>
    public string? ReplacementTypeOrMember { get; set; }

    /// <summary>
    /// The version when the <see cref="ObsoleteAttribute" /> on the member will change from a warning to an error. Must be convertible to a <see cref="Version"/>.
    /// </summary>
    public string? TreatAsErrorFromVersion { get; set; }

    /// <summary>
    /// The version when the obsolete member will be removed. Must be convertible to a <see cref="Version"/>.
    /// </summary>
    public string? RemoveInVersion { get; set; }

    /// <summary>
    /// The diagnostic ID to associate with the obsolete warning. Follows the convention of a short repository prefix followed by a zero-padded number, e.g. "NSB0001".
    /// This enables consumers to suppress the warning independently using <c>#pragma warning disable</c> or <c>&lt;NoWarn&gt;</c> with the specified ID,
    /// rather than the default <c>CS0618</c>.
    /// When set, this is propagated to the DiagnosticId property of the <see cref="ObsoleteAttribute" />.
    /// </summary>
    public string? DiagnosticId { get; set; }

    /// <summary>
    /// The URL for the help link that appears in IDE tooltips and build warnings.
    /// Can point to any documentation URL, such as a docs page or a GitHub issue.
    /// Can be used with or without <see cref="DiagnosticId" />; the two properties are independent.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When the format string contains the placeholder <c>{0}</c>, the compiler substitutes the diagnostic ID into it.
    /// If <see cref="DiagnosticId" /> is set, that value is used; otherwise the compiler falls back to the default obsolete diagnostic ID (<c>CS0618</c>).
    /// The placeholder receives the <b>entire</b> diagnostic ID string, not just the numeric portion.
    /// </para>
    /// <para>
    /// When the format string does <b>not</b> contain <c>{0}</c>, it is treated as a literal URL and used as-is.
    /// </para>
    /// <para>
    /// Use <c>{0}</c> when the URL page slug matches the diagnostic ID, for example a documentation site:
    /// <c>DiagnosticId = "NSB0001"</c> with <c>UrlFormat = "https://docs.particular.net/obsoletions/{0}"</c> produces <c>https://docs.particular.net/obsoletions/NSB0001</c>.
    /// </para>
    /// <para>
    /// Use a literal URL (no <c>{0}</c>) when linking directly to a specific GitHub issue,
    /// because GitHub issue URLs require a plain integer and the placeholder would receive the full diagnostic ID (e.g. <c>NSB0001</c>) instead of the issue number:
    /// <c>UrlFormat = "https://github.com/Particular/NServiceBus/issues/42"</c>.
    /// </para>
    /// </remarks>
    /// <seealso cref="DiagnosticId" />
    public string? UrlFormat { get; set; }
}