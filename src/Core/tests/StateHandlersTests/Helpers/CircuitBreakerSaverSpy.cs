using Core.CircuitBreakers.Context;
using Core.Storage;

namespace Core.Tests.StateHandlersTests.Helpers;

internal class CircuitBreakerUpdaterSpy : ICircuitBreakerContextUpdater
{
    internal CircuitBreakerState? UpdatedCircuitBreaker { get; private set; }

    public Task UpdateAsync(CircuitBreakerState state, CancellationToken token)
    {
        UpdatedCircuitBreaker = state;
        return Task.CompletedTask;
    }
}