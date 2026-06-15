using FieldOps.Application.Abstractions;
using FieldOps.Domain.Entities;
using FieldOps.Infrastructure.Mapping;
using FieldOps.Infrastructure.Persistence;

namespace FieldOps.Infrastructure.Repositories;

public sealed class WorkOrderRepository : IWorkOrderRepository
{
    private readonly LocalDatabase _database;

    public WorkOrderRepository(LocalDatabase database)
    {
        _database = database;
    }

    public async Task<IReadOnlyList<WorkOrder>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var records = await _database.Connection.Table<WorkOrderRecord>().ToListAsync();
        return records.Select(WorkOrderRecordMapper.ToEntity).ToList();
    }

    public async Task<WorkOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var record = await _database.Connection
            .Table<WorkOrderRecord>()
            .Where(r => r.Id == id)
            .FirstOrDefaultAsync();

        return record is null ? null : WorkOrderRecordMapper.ToEntity(record);
    }

    public async Task UpdateAsync(WorkOrder workOrder, CancellationToken cancellationToken = default)
    {
        await _database.Connection.InsertOrReplaceAsync(WorkOrderRecordMapper.ToRecord(workOrder));
    }

    public async Task<IReadOnlyList<WorkOrder>> GetPendingSyncAsync(CancellationToken cancellationToken = default)
    {
        var records = await _database.Connection
            .Table<WorkOrderRecord>()
            .Where(r => r.HasPendingSync)
            .ToListAsync();

        return records.Select(WorkOrderRecordMapper.ToEntity).ToList();
    }

    public async Task MarkSyncedAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        foreach (var id in ids)
        {
            var workOrder = await GetByIdAsync(id, cancellationToken);
            if (workOrder is null)
                continue;

            workOrder.MarkSynced();
            await UpdateAsync(workOrder, cancellationToken);
        }
    }

    public async Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default)
    {
        var count = await _database.Connection.Table<WorkOrderRecord>().CountAsync();
        return count == 0;
    }

    public async Task SeedAsync(IEnumerable<WorkOrder> workOrders, CancellationToken cancellationToken = default)
    {
        var records = workOrders.Select(WorkOrderRecordMapper.ToRecord).ToList();
        await _database.Connection.InsertAllAsync(records);
    }
}
