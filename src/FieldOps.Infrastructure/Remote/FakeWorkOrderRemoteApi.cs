using FieldOps.Application.Abstractions;
using FieldOps.Application.DTOs;

namespace FieldOps.Infrastructure.Remote;

public sealed class FakeWorkOrderRemoteApi : IWorkOrderRemoteApi
{
    private readonly FakeRemoteApiOptions _options;
    private readonly Random _random = new();

    public FakeWorkOrderRemoteApi(FakeRemoteApiOptions options)
    {
        _options = options;
    }

    public async Task<RemoteApiResult> PushChangesAsync(
        IReadOnlyList<WorkOrderDto> changes,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(_options.SimulatedDelayMs, cancellationToken);

        if (_options.ForceFailure || _random.NextDouble() < _options.FailureProbability)
            return RemoteApiResult.Failure("Unable to reach the server. Your changes are saved locally and will sync when connectivity is restored.");

        return RemoteApiResult.Success();
    }
}
