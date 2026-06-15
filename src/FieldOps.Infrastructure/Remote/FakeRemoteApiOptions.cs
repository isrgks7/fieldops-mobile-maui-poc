namespace FieldOps.Infrastructure.Remote;

public sealed class FakeRemoteApiOptions
{
    public int SimulatedDelayMs { get; set; } = 1500;
    public double FailureProbability { get; set; } = 0.15;
    public bool ForceFailure { get; set; } = false;
}
