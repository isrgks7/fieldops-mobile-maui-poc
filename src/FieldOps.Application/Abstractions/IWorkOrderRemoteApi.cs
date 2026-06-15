using FieldOps.Application.DTOs;

namespace FieldOps.Application.Abstractions;

public sealed class RemoteApiResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }

    public static RemoteApiResult Success() => new() { IsSuccess = true };
    public static RemoteApiResult Failure(string message) => new() { IsSuccess = false, ErrorMessage = message };
}

public interface IWorkOrderRemoteApi
{
    Task<RemoteApiResult> PushChangesAsync(
        IReadOnlyList<WorkOrderDto> changes,
        CancellationToken cancellationToken = default);
}
