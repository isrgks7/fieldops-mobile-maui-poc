using FieldOps.Application.Abstractions;
using FieldOps.Application.DTOs;
using FieldOps.Application.UseCases;
using FieldOps.Domain.Entities;
using FieldOps.Domain.Enums;
using FieldOps.Infrastructure;
using FieldOps.Infrastructure.Persistence;
using FieldOps.Infrastructure.Remote;
using Microsoft.Extensions.DependencyInjection;

namespace FieldOps.Application.Tests;

public class WorkOrderBusinessRulesTests
{
    [Fact]
    public void CannotCompleteWorkOrderWithoutTechnicianNotes()
    {
        var workOrder = CreateWorkOrder(notes: string.Empty);

        var error = workOrder.TryUpdateStatus(WorkOrderStatus.Completed);

        Assert.NotNull(error);
        Assert.Contains("notes", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CompletedWorkOrderCannotReturnToOpen()
    {
        var workOrder = CreateWorkOrder(status: WorkOrderStatus.Completed, notes: "Completed work.");

        var error = workOrder.TryUpdateStatus(WorkOrderStatus.Open);

        Assert.NotNull(error);
        Assert.Contains("Completed", error);
    }

    [Fact]
    public void UpdatingNotesMarksWorkOrderAsPendingSync()
    {
        var workOrder = CreateWorkOrder(hasPendingSync: false);

        var error = workOrder.TryUpdateTechnicianNotes("Updated notes.");

        Assert.Null(error);
        Assert.True(workOrder.HasPendingSync);
    }

    private static WorkOrder CreateWorkOrder(
        WorkOrderStatus status = WorkOrderStatus.Open,
        string notes = "Initial notes",
        bool hasPendingSync = false)
    {
        return new WorkOrder(
            Guid.NewGuid(),
            "WO-TEST",
            "Customer",
            "Address",
            "Title",
            "Description",
            WorkOrderPriority.Medium,
            status,
            DateTimeOffset.UtcNow.AddDays(1),
            notes,
            hasPendingSync,
            DateTimeOffset.UtcNow,
            status == WorkOrderStatus.Completed ? DateTimeOffset.UtcNow : null);
    }
}

public class GetDashboardSummaryUseCaseTests
{
    [Fact]
    public async Task DashboardSummaryCountsStatusesCorrectly()
    {
        var repository = new InMemoryWorkOrderRepository();
        await repository.SeedAsync(new[]
        {
            CreateWorkOrder(WorkOrderStatus.Open),
            CreateWorkOrder(WorkOrderStatus.Open),
            CreateWorkOrder(WorkOrderStatus.InProgress),
            CreateWorkOrder(WorkOrderStatus.Completed, hasPendingSync: true)
        });

        var useCase = new GetDashboardSummaryUseCase(repository, new InMemorySyncMetadataRepository());
        var result = await useCase.ExecuteAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(4, result.Value!.Total);
        Assert.Equal(2, result.Value.Open);
        Assert.Equal(1, result.Value.InProgress);
        Assert.Equal(1, result.Value.Completed);
        Assert.Equal(1, result.Value.PendingSync);
    }

    private static WorkOrder CreateWorkOrder(WorkOrderStatus status, bool hasPendingSync = false)
    {
        return new WorkOrder(
            Guid.NewGuid(),
            $"WO-{Guid.NewGuid():N}"[..8],
            "Customer",
            "Address",
            "Title",
            "Description",
            WorkOrderPriority.Medium,
            status,
            DateTimeOffset.UtcNow,
            "Notes",
            hasPendingSync,
            DateTimeOffset.UtcNow,
            status == WorkOrderStatus.Completed ? DateTimeOffset.UtcNow : null);
    }
}

public class GetWorkOrdersUseCaseTests
{
    [Fact]
    public async Task SearchAndFilterLogicWorks()
    {
        var repository = new InMemoryWorkOrderRepository();
        await repository.SeedAsync(new[]
        {
            CreateNamedWorkOrder("Alpha Corp", "Pump repair", WorkOrderStatus.Open),
            CreateNamedWorkOrder("Beta LLC", "Electrical check", WorkOrderStatus.InProgress),
            CreateNamedWorkOrder("Gamma Inc", "Pump inspection", WorkOrderStatus.Completed)
        });

        var useCase = new GetWorkOrdersUseCase(repository);

        var openResult = await useCase.ExecuteAsync(new WorkOrderQuery { Filter = WorkOrderListFilter.Open });
        Assert.Single(openResult.Value!);

        var searchResult = await useCase.ExecuteAsync(new WorkOrderQuery { SearchText = "pump" });
        Assert.Equal(2, searchResult.Value!.Count);
    }

    private static WorkOrder CreateNamedWorkOrder(string customer, string title, WorkOrderStatus status)
    {
        return new WorkOrder(
            Guid.NewGuid(),
            "WO-0001",
            customer,
            "Address",
            title,
            "Description",
            WorkOrderPriority.Medium,
            status,
            DateTimeOffset.UtcNow,
            "Notes",
            false,
            DateTimeOffset.UtcNow,
            null);
    }
}

public class SyncWorkOrdersUseCaseTests
{
    [Fact]
    public async Task SuccessfulSyncClearsPendingSyncFlag()
    {
        var repository = new InMemoryWorkOrderRepository();
        var workOrder = CreatePendingWorkOrder();
        await repository.SeedAsync(new[] { workOrder });

        var services = new ServiceCollection();
        services.AddSingleton<IWorkOrderRepository>(repository);
        services.AddSingleton<ISyncMetadataRepository, InMemorySyncMetadataRepository>();
        services.AddSingleton(new FakeRemoteApiOptions { FailureProbability = 0, SimulatedDelayMs = 0 });
        services.AddSingleton<IWorkOrderRemoteApi, FakeWorkOrderRemoteApi>();
        services.AddSingleton<SyncWorkOrdersUseCase>();

        var useCase = services.BuildServiceProvider().GetRequiredService<SyncWorkOrdersUseCase>();
        var result = await useCase.ExecuteAsync();

        Assert.True(result.IsSuccess);
        var updated = await repository.GetByIdAsync(workOrder.Id);
        Assert.NotNull(updated);
        Assert.False(updated!.HasPendingSync);
    }

    private static WorkOrder CreatePendingWorkOrder()
    {
        var workOrder = new WorkOrder(
            Guid.NewGuid(),
            "WO-SYNC",
            "Customer",
            "Address",
            "Title",
            "Description",
            WorkOrderPriority.High,
            WorkOrderStatus.InProgress,
            DateTimeOffset.UtcNow,
            "Notes",
            false,
            DateTimeOffset.UtcNow,
            null);

        workOrder.TryUpdateTechnicianNotes("Changed locally.");
        return workOrder;
    }
}

internal sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
{
    private readonly Dictionary<Guid, WorkOrder> _items = new();

    public Task<IReadOnlyList<WorkOrder>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<WorkOrder>>(_items.Values.ToList());

    public Task<WorkOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_items.TryGetValue(id, out var item) ? item : null);

    public Task UpdateAsync(WorkOrder workOrder, CancellationToken cancellationToken = default)
    {
        _items[workOrder.Id] = Clone(workOrder);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<WorkOrder>> GetPendingSyncAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<WorkOrder>>(_items.Values.Where(w => w.HasPendingSync).ToList());

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

    public Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_items.Count == 0);

    public Task SeedAsync(IEnumerable<WorkOrder> workOrders, CancellationToken cancellationToken = default)
    {
        foreach (var workOrder in workOrders)
            _items[workOrder.Id] = Clone(workOrder);

        return Task.CompletedTask;
    }

    private static WorkOrder Clone(WorkOrder source) => new(
        source.Id,
        source.WorkOrderNumber,
        source.CustomerName,
        source.CustomerAddress,
        source.Title,
        source.Description,
        source.Priority,
        source.Status,
        source.DueDate,
        source.TechnicianNotes,
        source.HasPendingSync,
        source.LastUpdatedAt,
        source.CompletedAt);
}

internal sealed class InMemorySyncMetadataRepository : ISyncMetadataRepository
{
    private DateTimeOffset? _lastSync;

    public Task<DateTimeOffset?> GetLastSyncAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_lastSync);

    public Task SetLastSyncAsync(DateTimeOffset syncTime, CancellationToken cancellationToken = default)
    {
        _lastSync = syncTime;
        return Task.CompletedTask;
    }
}
