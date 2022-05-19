using DCB.Core.CircuitBreakers.Context;
using DCB.Core.Storage;

namespace DCB.Core.Tests.StateHandlersTests.Helpers;

internal class CircuitBreakerUpdaterSpy : ICircuitBreakerContextUpdater
{
    internal CircuitBreakerContextSnapshot? UpdatedCircuitBreaker { get; private set; }

    public Task UpdateAsync(CircuitBreakerContextSnapshot snapshot, CancellationToken token)
    {
        UpdatedCircuitBreaker = snapshot;
        return Task.CompletedTask;
    }
}