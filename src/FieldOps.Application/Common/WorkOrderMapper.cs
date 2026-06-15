using FieldOps.Application.DTOs;
using FieldOps.Domain.Entities;

namespace FieldOps.Application.Common;

public static class WorkOrderMapper
{
    public static WorkOrderDto ToDto(WorkOrder workOrder) => new()
    {
        Id = workOrder.Id,
        WorkOrderNumber = workOrder.WorkOrderNumber,
        CustomerName = workOrder.CustomerName,
        CustomerAddress = workOrder.CustomerAddress,
        Title = workOrder.Title,
        Description = workOrder.Description,
        Priority = workOrder.Priority,
        Status = workOrder.Status,
        DueDate = workOrder.DueDate,
        TechnicianNotes = workOrder.TechnicianNotes,
        HasPendingSync = workOrder.HasPendingSync,
        LastUpdatedAt = workOrder.LastUpdatedAt,
        CompletedAt = workOrder.CompletedAt
    };
}
