using FieldOps.Domain.Entities;

namespace FieldOps.Application.Abstractions;

public interface ISeedDataProvider
{
    IReadOnlyList<WorkOrder> GetSeedWorkOrders();
}
