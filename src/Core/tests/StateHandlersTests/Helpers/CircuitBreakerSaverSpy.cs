namespace Core.Tests.StateHandlersTests.Helpers;

internal class CircuitBreakerUpdaterSpy : ICircuitBreakerContextUpdater
{
    internal CircuitBreakerSnapshot? UpdatedCircuitBreaker { get; private set; }

    public Task UpdateAsync(CircuitBreakerSnapshot snapshot, CancellationToken token)
    {
        UpdatedCircuitBreaker = snapshot;
        return Task.CompletedTask;
    }
}