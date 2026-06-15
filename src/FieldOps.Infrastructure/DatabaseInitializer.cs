using FieldOps.Application.Abstractions;
using FieldOps.Application.UseCases;
using FieldOps.Infrastructure.Persistence;

namespace FieldOps.Infrastructure;

public sealed class DatabaseInitializer : IDatabaseInitializer
{
    private readonly LocalDatabase _database;
    private readonly SeedInitialDataUseCase _seedInitialDataUseCase;

    public DatabaseInitializer(LocalDatabase database, SeedInitialDataUseCase seedInitialDataUseCase)
    {
        _database = database;
        _seedInitialDataUseCase = seedInitialDataUseCase;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await _database.InitializeAsync();
        await _seedInitialDataUseCase.ExecuteAsync(cancellationToken);
    }
}
