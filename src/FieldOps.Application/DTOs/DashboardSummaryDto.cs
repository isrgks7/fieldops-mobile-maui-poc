namespace FieldOps.Application.DTOs;

public sealed class DashboardSummaryDto
{
    public int Total { get; init; }
    public int Open { get; init; }
    public int InProgress { get; init; }
    public int Completed { get; init; }
    public int PendingSync { get; init; }
    public DateTimeOffset? LastSyncAt { get; init; }
}
