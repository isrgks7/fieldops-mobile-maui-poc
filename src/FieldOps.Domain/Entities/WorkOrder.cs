using FieldOps.Domain.Enums;

namespace FieldOps.Domain.Entities;

public sealed class WorkOrder
{
    public const int MaxTechnicianNotesLength = 500;

    public Guid Id { get; private set; }
    public string WorkOrderNumber { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerAddress { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public WorkOrderPriority Priority { get; private set; }
    public WorkOrderStatus Status { get; private set; }
    public DateTimeOffset DueDate { get; private set; }
    public string TechnicianNotes { get; private set; } = string.Empty;
    public bool HasPendingSync { get; private set; }
    public DateTimeOffset LastUpdatedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    public WorkOrder(
        Guid id,
        string workOrderNumber,
        string customerName,
        string customerAddress,
        string title,
        string description,
        WorkOrderPriority priority,
        WorkOrderStatus status,
        DateTimeOffset dueDate,
        string technicianNotes,
        bool hasPendingSync,
        DateTimeOffset lastUpdatedAt,
        DateTimeOffset? completedAt)
    {
        Id = id;
        WorkOrderNumber = workOrderNumber;
        CustomerName = customerName;
        CustomerAddress = customerAddress;
        Title = title;
        Description = description;
        Priority = priority;
        Status = status;
        DueDate = dueDate;
        TechnicianNotes = technicianNotes;
        HasPendingSync = hasPendingSync;
        LastUpdatedAt = lastUpdatedAt;
        CompletedAt = completedAt;
    }

    public static WorkOrder CreateNew(
        string workOrderNumber,
        string customerName,
        string customerAddress,
        string title,
        string description,
        WorkOrderPriority priority,
        WorkOrderStatus status,
        DateTimeOffset dueDate,
        string technicianNotes = "")
    {
        var now = DateTimeOffset.UtcNow;
        return new WorkOrder(
            Guid.NewGuid(),
            workOrderNumber,
            customerName,
            customerAddress,
            title,
            description,
            priority,
            status,
            dueDate,
            technicianNotes,
            false,
            now,
            status == WorkOrderStatus.Completed ? now : null);
    }

    public string? TryUpdateStatus(WorkOrderStatus newStatus)
    {
        if (Status == WorkOrderStatus.Completed && newStatus == WorkOrderStatus.Open)
            return "Completed work orders cannot be moved back to Open.";

        if (newStatus == WorkOrderStatus.Completed && string.IsNullOrWhiteSpace(TechnicianNotes))
            return "Technician notes are required before marking as completed.";

        Status = newStatus;
        if (newStatus == WorkOrderStatus.Completed)
            CompletedAt = DateTimeOffset.UtcNow;

        MarkLocallyUpdated();
        return null;
    }

    public string? TryUpdateTechnicianNotes(string notes)
    {
        if (notes.Length > MaxTechnicianNotesLength)
            return $"Technician notes cannot exceed {MaxTechnicianNotesLength} characters.";

        TechnicianNotes = notes;
        MarkLocallyUpdated();
        return null;
    }

    public void MarkSynced()
    {
        HasPendingSync = false;
        LastUpdatedAt = DateTimeOffset.UtcNow;
    }

    private void MarkLocallyUpdated()
    {
        HasPendingSync = true;
        LastUpdatedAt = DateTimeOffset.UtcNow;
    }
}
