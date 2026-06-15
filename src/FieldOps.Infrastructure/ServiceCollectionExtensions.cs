using FieldOps.Application.Abstractions;
using FieldOps.Application.UseCases;
using FieldOps.Infrastructure.Persistence;
using FieldOps.Infrastructure.Remote;
using FieldOps.Infrastructure.Repositories;
using FieldOps.Infrastructure.SeedData;
using Microsoft.Extensions.DependencyInjection;
using SQLitePCL;

namespace FieldOps.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFieldOpsInfrastructure(
        this IServiceCollection services,
        IDatabasePathProvider? databasePathProvider = null)
    {
        Batteries_V2.Init();

        services.AddSingleton(databasePathProvider ?? new FileDatabasePathProvider());
        services.AddSingleton(sp =>
        {
            var pathProvider = sp.GetRequiredService<IDatabasePathProvider>();
            return new LocalDatabase(pathProvider.GetDatabasePath());
        });

        services.AddSingleton<IWorkOrderRepository, WorkOrderRepository>();
        services.AddSingleton<ISyncMetadataRepository, SyncMetadataRepository>();
        services.AddSingleton<ISeedDataProvider, WorkOrderSeedDataProvider>();
        services.AddSingleton<FakeRemoteApiOptions>();
        services.AddSingleton<IWorkOrderRemoteApi, FakeWorkOrderRemoteApi>();
        services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();

        return services;
    }

    public static IServiceCollection AddFieldOpsApplication(this IServiceCollection services)
    {
        services.AddSingleton<GetDashboardSummaryUseCase>();
        services.AddSingleton<GetWorkOrdersUseCase>();
        services.AddSingleton<GetWorkOrderByIdUseCase>();
        services.AddSingleton<UpdateWorkOrderStatusUseCase>();
        services.AddSingleton<UpdateTechnicianNotesUseCase>();
        services.AddSingleton<SyncWorkOrdersUseCase>();
        services.AddSingleton<SeedInitialDataUseCase>();

        return services;
    }
}
