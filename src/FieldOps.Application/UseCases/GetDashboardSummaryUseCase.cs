using FieldOps.Application.Abstractions;
using FieldOps.Application.Common;
using FieldOps.Application.DTOs;

namespace FieldOps.Application.UseCases;

public sealed class GetDashboardSummaryUseCase
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly ISyncMetadataRepository _syncMetadataRepository;

    public GetDashboardSummaryUseCase(
        IWorkOrderRepository workOrderRepository,
        ISyncMetadataRepository syncMetadataRepository)
    {
        _workOrderRepository = workOrderRepository;
        _syncMetadataRepository = syncMetadataRepository;
    }

    public async Task<Result<DashboardSummaryDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var workOrders = await _workOrderRepository.GetAllAsync(cancellationToken);
        var lastSync = await _syncMetadataRepository.GetLastSyncAsync(cancellationToken);

        var summary = new DashboardSummaryDto
        {
            Total = workOrders.Count,
            Open = workOrders.Count(w => w.Status == Domain.Enums.WorkOrderStatus.Open),
            InProgress = workOrders.Count(w => w.Status == Domain.Enums.WorkOrderStatus.InProgress),
            Completed = workOrders.Count(w => w.Status == Domain.Enums.WorkOrderStatus.Completed),
            PendingSync = workOrders.Count(w => w.HasPendingSync),
            LastSyncAt = lastSync
        };

        return Result<DashboardSummaryDto>.Success(summary);
    }
}
