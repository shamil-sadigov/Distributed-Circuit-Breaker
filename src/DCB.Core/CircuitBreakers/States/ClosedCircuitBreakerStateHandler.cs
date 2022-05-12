using DCB.Core.CircuitBreakerOption;

namespace DCB.Core.CircuitBreakers.States;

internal sealed class ClosedCircuitBreakerStateHandler:ICircuitBreakerStateHandler
{
    public async Task<TResult> HandleAsync<TResult>(
        CircuitBreakerOptions options,
        ICircuitBreakerStore store,
        Func<Task<TResult>> action, 
        CircuitBreakerData data)
    {
    }
}