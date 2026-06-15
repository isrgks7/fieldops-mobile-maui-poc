using SQLite;

namespace FieldOps.Infrastructure.Persistence;

[Table("WorkOrders")]
public sealed class WorkOrderRecord
{
    [PrimaryKey]
    public Guid Id { get; set; }

    [Indexed]
    public string WorkOrderNumber { get; set; } = string.Empty;

    public string CustomerName { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; }
    public int Status { get; set; }
    public DateTimeOffset DueDate { get; set; }
    public string TechnicianNotes { get; set; } = string.Empty;
    public bool HasPendingSync { get; set; }
    public DateTimeOffset LastUpdatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
