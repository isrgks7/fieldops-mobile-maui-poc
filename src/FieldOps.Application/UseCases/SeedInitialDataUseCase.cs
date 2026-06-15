using FieldOps.Application.Abstractions;
using FieldOps.Application.Common;

namespace FieldOps.Application.UseCases;

public sealed class SeedInitialDataUseCase
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly ISeedDataProvider _seedDataProvider;

    public SeedInitialDataUseCase(
        IWorkOrderRepository workOrderRepository,
        ISeedDataProvider seedDataProvider)
    {
        _workOrderRepository = workOrderRepository;
        _seedDataProvider = seedDataProvider;
    }

    public async Task<Result> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        if (!await _workOrderRepository.IsEmptyAsync(cancellationToken))
            return Result.Success();

        var seedData = _seedDataProvider.GetSeedWorkOrders();
        await _workOrderRepository.SeedAsync(seedData, cancellationToken);
        return Result.Success();
    }
}
