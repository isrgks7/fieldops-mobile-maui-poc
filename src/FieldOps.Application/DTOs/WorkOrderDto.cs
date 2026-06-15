using FieldOps.Domain.Enums;

namespace FieldOps.Application.DTOs;

public sealed class WorkOrderDto
{
    public Guid Id { get; init; }
    public string WorkOrderNumber { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerAddress { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public WorkOrderPriority Priority { get; init; }
    public WorkOrderStatus Status { get; init; }
    public DateTimeOffset DueDate { get; init; }
    public string TechnicianNotes { get; init; } = string.Empty;
    public bool HasPendingSync { get; init; }
    public DateTimeOffset LastUpdatedAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
}
