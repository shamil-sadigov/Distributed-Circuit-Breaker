using DCB.Core.CircuitBreakerOption;

namespace DCB.Core.CircuitBreakers.States;

internal sealed class OpenCircuitBreakerState:ICircuitBreakerState
{
    public Task<TResult> ExecuteAsync<TResult>(CircuitBreakerOptions options, ICircuitBreakerStore store)
    {
        throw new NotImplementedException();
    }
}