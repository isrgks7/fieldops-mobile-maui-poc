using FieldOps.Application.Abstractions;
using FieldOps.Application.Common;
using FieldOps.Application.DTOs;
using FieldOps.Domain.Enums;

namespace FieldOps.Application.UseCases;

public sealed class UpdateWorkOrderStatusUseCase
{
    private readonly IWorkOrderRepository _workOrderRepository;

    public UpdateWorkOrderStatusUseCase(IWorkOrderRepository workOrderRepository)
    {
        _workOrderRepository = workOrderRepository;
    }

    public async Task<Result<WorkOrderDto>> ExecuteAsync(
        Guid id,
        WorkOrderStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(id, cancellationToken);
        if (workOrder is null)
            return Result<WorkOrderDto>.Failure("Work order not found.");

        var error = workOrder.TryUpdateStatus(newStatus);
        if (error is not null)
            return Result<WorkOrderDto>.Failure(error);

        await _workOrderRepository.UpdateAsync(workOrder, cancellationToken);
        return Result<WorkOrderDto>.Success(WorkOrderMapper.ToDto(workOrder));
    }
}
