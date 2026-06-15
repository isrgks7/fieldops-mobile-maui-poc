using FieldOps.Domain.Entities;

namespace FieldOps.Application.Abstractions;

public interface IWorkOrderRepository
{
    Task<IReadOnlyList<WorkOrder>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WorkOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkOrder workOrder, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkOrder>> GetPendingSyncAsync(CancellationToken cancellationToken = default);
    Task MarkSyncedAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default);
    Task SeedAsync(IEnumerable<WorkOrder> workOrders, CancellationToken cancellationToken = default);
}
