using System.Runtime.CompilerServices;
using DCB.Core.CircuitBreakerOption;



namespace DCB.Core.CircuitBreakers.States;

internal sealed class HalfOpenCircuitBreakerStateHandler:ICircuitBreakerStateHandler
{
    public Task<TResult> HandleAsync<TResult>(CircuitBreakerOptions options, ICircuitBreakerStore store,
        Func<Task<TResult>> action, CircuitBreakerData data)
    {
        throw new NotImplementedException();
    }
}