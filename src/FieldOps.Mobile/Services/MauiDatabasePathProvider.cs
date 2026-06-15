using FieldOps.Infrastructure.Persistence;

namespace FieldOps.Mobile.Services;

/// <summary>
/// Uses MAUI's platform-safe app data directory for SQLite on Android, iOS, and Windows.
/// </summary>
public sealed class MauiDatabasePathProvider : IDatabasePathProvider
{
    public string GetDatabasePath()
    {
        var folder = Path.Combine(FileSystem.AppDataDirectory, "FieldOpsMobile");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "fieldops.db");
    }
}
