using FieldOps.Domain.Entities;
using FieldOps.Domain.Enums;
using FieldOps.Infrastructure.Persistence;

namespace FieldOps.Infrastructure.Mapping;

internal static class WorkOrderRecordMapper
{
    public static WorkOrder ToEntity(WorkOrderRecord record) => new(
        record.Id,
        record.WorkOrderNumber,
        record.CustomerName,
        record.CustomerAddress,
        record.Title,
        record.Description,
        (WorkOrderPriority)record.Priority,
        (WorkOrderStatus)record.Status,
        record.DueDate,
        record.TechnicianNotes,
        record.HasPendingSync,
        record.LastUpdatedAt,
        record.CompletedAt);

    public static WorkOrderRecord ToRecord(WorkOrder entity) => new()
    {
        Id = entity.Id,
        WorkOrderNumber = entity.WorkOrderNumber,
        CustomerName = entity.CustomerName,
        CustomerAddress = entity.CustomerAddress,
        Title = entity.Title,
        Description = entity.Description,
        Priority = (int)entity.Priority,
        Status = (int)entity.Status,
        DueDate = entity.DueDate,
        TechnicianNotes = entity.TechnicianNotes,
        HasPendingSync = entity.HasPendingSync,
        LastUpdatedAt = entity.LastUpdatedAt,
        CompletedAt = entity.CompletedAt
    };
}
