using DCB.Core.CircuitBreakers;

namespace DCB.Core;

public interface ICircuitBreakerStore
{
    public Task InitializeAsync();
    


    // TODO: Add methods    
    CircuitBreakerStateEnum GetState(string circuitBreakerName);
}