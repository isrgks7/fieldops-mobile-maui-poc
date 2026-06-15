using FieldOps.Application.Abstractions;
using FieldOps.Application.Common;
using FieldOps.Application.DTOs;

namespace FieldOps.Application.UseCases;

public sealed class SyncWorkOrdersUseCase
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly ISyncMetadataRepository _syncMetadataRepository;
    private readonly IWorkOrderRemoteApi _remoteApi;

    public SyncWorkOrdersUseCase(
        IWorkOrderRepository workOrderRepository,
        ISyncMetadataRepository syncMetadataRepository,
        IWorkOrderRemoteApi remoteApi)
    {
        _workOrderRepository = workOrderRepository;
        _syncMetadataRepository = syncMetadataRepository;
        _remoteApi = remoteApi;
    }

    public async Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var pending = await _workOrderRepository.GetPendingSyncAsync(cancellationToken);
        if (pending.Count == 0)
        {
            await _syncMetadataRepository.SetLastSyncAsync(DateTimeOffset.UtcNow, cancellationToken);
            return Result.Success();
        }

        var changes = pending.Select(WorkOrderMapper.ToDto).ToList();
        var remoteResult = await _remoteApi.PushChangesAsync(changes, cancellationToken);
        if (!remoteResult.IsSuccess)
            return Result.Failure(remoteResult.ErrorMessage ?? "Sync failed. Please try again.");

        await _workOrderRepository.MarkSyncedAsync(pending.Select(w => w.Id), cancellationToken);
        await _syncMetadataRepository.SetLastSyncAsync(DateTimeOffset.UtcNow, cancellationToken);
        return Result.Success();
    }
}
