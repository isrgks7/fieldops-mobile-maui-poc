using FieldOps.Application.Abstractions;
using FieldOps.Application.Common;
using FieldOps.Application.DTOs;

namespace FieldOps.Application.UseCases;

public sealed class GetWorkOrderByIdUseCase
{
    private readonly IWorkOrderRepository _workOrderRepository;

    public GetWorkOrderByIdUseCase(IWorkOrderRepository workOrderRepository)
    {
        _workOrderRepository = workOrderRepository;
    }

    public async Task<Result<WorkOrderDto>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(id, cancellationToken);
        if (workOrder is null)
            return Result<WorkOrderDto>.Failure("Work order not found.");

        return Result<WorkOrderDto>.Success(WorkOrderMapper.ToDto(workOrder));
    }
}
