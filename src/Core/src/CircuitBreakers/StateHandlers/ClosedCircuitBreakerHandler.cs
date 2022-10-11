using Core.CircuitBreakerOption;
using Core.CircuitBreakers.Context;
using Core.Storage;

namespace Core.CircuitBreakers.StateHandlers;

// TODO: Add logging

internal sealed class ClosedCircuitBreakerHandler : ICircuitBreakerStateHandler
{
    private readonly ICircuitBreakerContextUpdater _contextUpdater;

    public ClosedCircuitBreakerHandler(ICircuitBreakerContextUpdater contextUpdater)
    {
        _contextUpdater = contextUpdater;
    }

    public async Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CircuitBreakerSettings settings,
        CancellationToken token)
    {
        try
        {
            var result = await action(token).ConfigureAwait(false);

            if (!settings.CanHandleResult(result))
                return result;

            circuitBreaker.Failed();
            await SaveAsync(circuitBreaker.CreateSnapshot(), token);
            return result;
        }
        catch (Exception ex)
        {
            if (!settings.CanHandleException(ex))
                throw;

            circuitBreaker.Failed();
            await SaveAsync(circuitBreaker.CreateSnapshot(), token);
            throw;
        }
    }

    public bool CanHandle(CircuitBreakerContext context)
    {
        return context.State is CircuitBreakerState.Closed;
    }

    private async Task SaveAsync(CircuitBreakerSnapshot circuitBreakerSnapshot, CancellationToken token)
    {
        await _contextUpdater.UpdateAsync(circuitBreakerSnapshot, token).ConfigureAwait(false);
    }
    
}