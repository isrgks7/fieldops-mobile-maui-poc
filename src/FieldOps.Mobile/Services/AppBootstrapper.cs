using FieldOps.Application.Abstractions;

namespace FieldOps.Mobile.Services;

/// <summary>
/// Starts database initialization without blocking MAUI startup on the UI thread.
/// </summary>
public sealed class AppBootstrapper
{
    public Task InitializationTask { get; }

    public AppBootstrapper(IDatabaseInitializer databaseInitializer)
    {
        InitializationTask = InitializeSafeAsync(databaseInitializer);
    }

    private static async Task InitializeSafeAsync(IDatabaseInitializer databaseInitializer)
    {
        try
        {
            await databaseInitializer.InitializeAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex}");
            throw;
        }
    }
}
