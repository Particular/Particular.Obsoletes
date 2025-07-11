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
}