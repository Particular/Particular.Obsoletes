namespace Particular.Obsoletes;

using Microsoft.CodeAnalysis;

public static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor DroppedTask = new(
        id: DiagnosticIds.DroppedTask,
        title: "Tasks returned from expressions should be returned, awaited, or assigned to a variable",
        messageFormat: "Return, await, or assign the task to a variable",
        category: "Code",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
