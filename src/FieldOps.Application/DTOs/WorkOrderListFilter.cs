using FieldOps.Domain.Enums;

namespace FieldOps.Application.DTOs;

public enum WorkOrderListFilter
{
    All,
    Open,
    InProgress,
    Completed
}

public sealed class WorkOrderQuery
{
    public WorkOrderListFilter Filter { get; init; } = WorkOrderListFilter.All;
    public string? SearchText { get; init; }
}
