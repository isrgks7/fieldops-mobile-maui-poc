using FieldOps.Application.Abstractions;
using FieldOps.Application.UseCases;
using FieldOps.Infrastructure;
using FieldOps.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace FieldOps.Infrastructure.Tests;

public class WorkOrderRepositoryTests : IDisposable
{
    private readonly string _databasePath;
    private readonly ServiceProvider _serviceProvider;

    public WorkOrderRepositoryTests()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"fieldops-test-{Guid.NewGuid():N}.db");
        var services = new ServiceCollection();
        services.AddFieldOpsInfrastructure(new TestDatabasePathProvider(_databasePath));
        services.AddFieldOpsApplication();
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task RepositoryReturnsSeededDataWhenDatabaseIsEmpty()
    {
        var initializer = _serviceProvider.GetRequiredService<IDatabaseInitializer>();
        await initializer.InitializeAsync();

        var repository = _serviceProvider.GetRequiredService<IWorkOrderRepository>();
        var workOrders = await repository.GetAllAsync();

        Assert.NotEmpty(workOrders);
        Assert.True(workOrders.Count >= 6);
        Assert.Contains(workOrders, w => w.Priority == Domain.Enums.WorkOrderPriority.Critical);
    }

    [Fact]
    public async Task SeedInitialDataDoesNotRunWhenDatabaseAlreadyHasRecords()
    {
        var initializer = _serviceProvider.GetRequiredService<IDatabaseInitializer>();
        await initializer.InitializeAsync();

        var repository = _serviceProvider.GetRequiredService<IWorkOrderRepository>();
        var firstCount = (await repository.GetAllAsync()).Count;

        var seedUseCase = _serviceProvider.GetRequiredService<SeedInitialDataUseCase>();
        await seedUseCase.ExecuteAsync();

        var secondCount = (await repository.GetAllAsync()).Count;
        Assert.Equal(firstCount, secondCount);
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
        if (File.Exists(_databasePath))
            File.Delete(_databasePath);
    }
}
