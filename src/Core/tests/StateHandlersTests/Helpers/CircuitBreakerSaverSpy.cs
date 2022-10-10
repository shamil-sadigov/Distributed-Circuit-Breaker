using Core.CircuitBreakers.Context;
using Core.Storage;

namespace Core.Tests.StateHandlersTests.Helpers;

internal class CircuitBreakerUpdaterSpy : ICircuitBreakerContextUpdater
{
    internal CircuitBreakerContextSnapshot? UpdatedCircuitBreaker { get; private set; }

    public Task UpdateAsync(CircuitBreakerContextSnapshot snapshot, CancellationToken token)
    {
        UpdatedCircuitBreaker = snapshot;
        return Task.CompletedTask;
    }
}