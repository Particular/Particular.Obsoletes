#nullable enable

namespace Particular.Obsoletes;

using System;
using System.Diagnostics;

[Conditional("PARTICULAR_OBSOLETES")]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
sealed class ObsoleteExAttribute : Attribute
{
    public string? Message { get; set; }

    public string? TreatAsErrorFromVersion { get; set; }

    public string? RemoveInVersion { get; set; }

    public string? ReplacementTypeOrMember { get; set; }
}