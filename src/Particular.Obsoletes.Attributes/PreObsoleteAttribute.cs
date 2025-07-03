#nullable enable

namespace Particular.Obsoletes;

using System;
using System.Diagnostics;

/// <summary>
///
/// </summary>
/// <param name="contextUrl"></param>
[Conditional("PARTICULAR_OBSOLETES")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
public sealed class PreObsoleteAttribute(string contextUrl) : Attribute
{
    /// <summary>
    ///
    /// </summary>
    public string ContextUrl { get; } = contextUrl;

    /// <summary>
    ///
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    ///
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    ///
    /// </summary>
    public string? ReplacementTypeOrMember { get; set; }
}