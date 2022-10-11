using Core.Context;
using Core.Settings;
using Core.Storage;

namespace Core.StateHandlers;

// TODO: Add logging

internal sealed class ClosedCircuitBreakerHandler : ICircuitBreakerStateHandler
{
    public async Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CancellationToken token)
    {
        try
        {
            var result = await action(token).ConfigureAwait(false);

            if (!circuitBreaker.CanHandleResult(result))
                return result;

            circuitBreaker.Failed();
            return result;
        }
        catch (Exception ex)
        {
            if (circuitBreaker.CanHandleException(ex))
                circuitBreaker.Failed();
            
            throw;
        }
    }

    public bool CanHandle(CircuitBreakerState state) => state is CircuitBreakerState.Closed;
}