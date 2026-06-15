using FieldOps.Infrastructure;
using FieldOps.Infrastructure.Remote;
using FieldOps.Mobile.Pages;
using FieldOps.Mobile.Services;
using FieldOps.Mobile.ViewModels;
using Microsoft.Extensions.Logging;

namespace FieldOps.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddFieldOpsInfrastructure(new MauiDatabasePathProvider());
        // Deterministic demo sync: no random failures, shorter simulated latency.
        builder.Services.AddSingleton(new FakeRemoteApiOptions
        {
            FailureProbability = 0,
            ForceFailure = false,
            SimulatedDelayMs = 800
        });
        builder.Services.AddFieldOpsApplication();
        builder.Services.AddSingleton<AppBootstrapper>();

        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<WorkOrdersViewModel>();
        builder.Services.AddTransient<WorkOrderDetailViewModel>();

        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<WorkOrdersPage>();
        builder.Services.AddTransient<WorkOrderDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
