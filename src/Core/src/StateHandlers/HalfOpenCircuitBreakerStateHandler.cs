using Core.Context;
using Core.Settings;
using Core.Storage;

namespace Core.StateHandlers;

// TODO: Add logging
internal sealed class HalfOpenCircuitBreakerStateHandler : ICircuitBreakerStateHandler
{
    public async Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CircuitBreakerSettings settings, CancellationToken token)
    {
        try
        {
            var result = await action(token).ConfigureAwait(false);

            if (settings.CanHandleResult(result))
                circuitBreaker.Failed();
            else
                circuitBreaker.Reset();

            return result;
        }
        catch (Exception ex)
        {
            if (settings.CanHandleException(ex))
                circuitBreaker.Failed();
            
            throw;
        }
    }

    public bool CanHandle(CircuitBreakerState state) => state is CircuitBreakerState.HalfOpen;
}