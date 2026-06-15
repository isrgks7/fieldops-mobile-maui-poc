using SQLite;

namespace FieldOps.Infrastructure.Persistence;

public sealed class LocalDatabase : IDisposable
{
    private readonly SQLiteAsyncConnection _connection;

    public LocalDatabase(string databasePath)
    {
        _connection = new SQLiteAsyncConnection(databasePath);
    }

    public async Task InitializeAsync()
    {
        await _connection.CreateTableAsync<WorkOrderRecord>();
        await _connection.CreateTableAsync<SyncMetadataRecord>();
    }

    public SQLiteAsyncConnection Connection => _connection;

    public void Dispose()
    {
        _connection.GetConnection().Close();
    }
}
