using DCB.Core.CircuitBreakers.States;

namespace DCB.Core.Tests.StateHandlers.Helpers;

internal class CircuitBreakerSaverSpy:ICircuitBreakerContextSaver
{
    internal CircuitBreakerContextSnapshot? SavedCircuitBreaker { get; private set; }
    
    public Task SaveAsync(CircuitBreakerContextSnapshot snapshot)
    {
        SavedCircuitBreaker = snapshot;
        return Task.CompletedTask;
    }
}