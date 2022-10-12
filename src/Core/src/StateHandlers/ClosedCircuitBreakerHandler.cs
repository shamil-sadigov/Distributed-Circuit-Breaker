using Core.Context;
using Core.Helpers;

namespace Core.StateHandlers;

internal sealed class ClosedCircuitBreakerHandler : ICircuitBreakerStateHandler
{
    public async Task<TResult> HandleAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CancellationToken token)
    {
        circuitBreaker.EnsureStateIs(CircuitBreakerState.Closed);
        
        token.ThrowIfCancellationRequested();
        
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