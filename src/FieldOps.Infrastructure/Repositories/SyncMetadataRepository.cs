using FieldOps.Application.Abstractions;
using FieldOps.Infrastructure.Persistence;

namespace FieldOps.Infrastructure.Repositories;

public sealed class SyncMetadataRepository : ISyncMetadataRepository
{
    private readonly LocalDatabase _database;

    public SyncMetadataRepository(LocalDatabase database)
    {
        _database = database;
    }

    public async Task<DateTimeOffset?> GetLastSyncAsync(CancellationToken cancellationToken = default)
    {
        var record = await _database.Connection
            .Table<SyncMetadataRecord>()
            .FirstOrDefaultAsync();

        return record?.LastSyncAt;
    }

    public async Task SetLastSyncAsync(DateTimeOffset syncTime, CancellationToken cancellationToken = default)
    {
        var record = await _database.Connection
            .Table<SyncMetadataRecord>()
            .FirstOrDefaultAsync();

        if (record is null)
        {
            record = new SyncMetadataRecord { LastSyncAt = syncTime };
            await _database.Connection.InsertAsync(record);
            return;
        }

        record.LastSyncAt = syncTime;
        await _database.Connection.UpdateAsync(record);
    }
}
