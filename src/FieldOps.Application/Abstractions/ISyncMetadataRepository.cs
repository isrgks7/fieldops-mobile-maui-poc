namespace FieldOps.Application.Abstractions;

public interface ISyncMetadataRepository
{
    Task<DateTimeOffset?> GetLastSyncAsync(CancellationToken cancellationToken = default);
    Task SetLastSyncAsync(DateTimeOffset syncTime, CancellationToken cancellationToken = default);
}
