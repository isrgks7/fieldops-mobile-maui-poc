namespace FieldOps.Infrastructure.Persistence;

public interface IDatabasePathProvider
{
    string GetDatabasePath();
}

public sealed class FileDatabasePathProvider : IDatabasePathProvider
{
    public string GetDatabasePath()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FieldOpsMobile");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, "fieldops.db");
    }
}

public sealed class TestDatabasePathProvider : IDatabasePathProvider
{
    private readonly string _path;

    public TestDatabasePathProvider(string path)
    {
        _path = path;
    }

    public string GetDatabasePath() => _path;
}
