#nullable enable

namespace Particular.Obsoletes;

using System;
using System.Diagnostics;

/// <summary>
///
/// </summary>
[Conditional("PARTICULAR_OBSOLETES")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
public sealed class ObsoleteMetadataAttribute : Attribute
{
    /// <summary>
    ///
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    ///
    /// </summary>
    public string? ReplacementTypeOrMember { get; set; }

    /// <summary>
    ///
    /// </summary>
    public string? TreatAsErrorFromVersion { get; set; }

    /// <summary>
    ///
    /// </summary>
    public string? RemoveInVersion { get; set; }
}