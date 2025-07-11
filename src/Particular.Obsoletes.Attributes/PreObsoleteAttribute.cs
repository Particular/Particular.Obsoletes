namespace Particular.Obsoletes;

using System;
using System.Diagnostics;

/// <summary>
/// Meant for staging future obsoletes.
/// </summary>
/// <param name="contextUrl">A link to a GitHub issue that provides context for why the future obsoletion is required.</param>
[Conditional("PARTICULAR_OBSOLETES")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
public sealed class PreObsoleteAttribute(string contextUrl) : Attribute
{
    /// <summary>
    ///  A link to a GitHub issue that provides context for why the future obsoletion is required.
    /// </summary>
    public string ContextUrl { get; } = contextUrl;

    /// <summary>
    /// A summary of the context provided in the issue that <see cref="ContextUrl" /> points to.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// The text string that describes alternative workarounds.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// A value pointing to the name of the replacement member if available.
    /// </summary>
    public string? ReplacementTypeOrMember { get; set; }
}