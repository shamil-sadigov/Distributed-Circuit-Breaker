using DCB.Core.CircuitBreakers.States;

namespace DCB.Core.Tests;

internal class CircuitBreakerSaverSpy:ICircuitBreakerContextSaver
{
    internal CircuitBreakerContext? SavedCircuitBreaker { get; private set; }
    
    public Task SaveAsync(CircuitBreakerContext context)
    {
        SavedCircuitBreaker = context;
        
        return Task.CompletedTask;
    }
}