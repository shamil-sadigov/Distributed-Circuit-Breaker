using Core.CircuitBreakerOption;
using Core.CircuitBreakers.Context;
using Core.Exceptions;
using Core.Storage;

namespace Core.CircuitBreakers.StateHandlers;

// TODO: Add logging
internal sealed class HalfOpenCircuitBreakerStateHandler : ICircuitBreakerStateHandler
{
    private readonly ICircuitBreakerContextUpdater _contextUpdater;
    private readonly ISystemClock _systemClock;

    public HalfOpenCircuitBreakerStateHandler(
        ISystemClock systemClock,
        ICircuitBreakerContextUpdater contextUpdater)
    {
        _systemClock = systemClock;
        _contextUpdater = contextUpdater;
    }

    public async Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CircuitBreakerOptions options, CancellationToken token)
    {
        EnsureCircuitBreakerIsHalfOpen(circuitBreaker);

        try
        {
            var result = await action(token).ConfigureAwait(false);
            
            if (!options.CanHandleResult(result))
            {
                circuitBreaker.Close(_systemClock.GetCurrentTime());
                await SaveAsync(circuitBreaker, token);
                return result;
            }

            circuitBreaker.Failed(_systemClock.GetCurrentTime());
            await SaveAsync(circuitBreaker, token);
            return result;
        }
        catch (Exception ex)
        {
            if (!options.CanHandleException(ex))
                throw;

            circuitBreaker.Failed(_systemClock.GetCurrentTime());
            await SaveAsync(circuitBreaker, token);
            throw;
        }
    }

    public bool CanHandle(CircuitBreakerContext context)
    {
        return context.State == CircuitBreakerState.HalfOpen;
    }

    private async Task SaveAsync(CircuitBreakerContext circuitBreaker, CancellationToken token)
    {
        var snapshot = circuitBreaker.GetSnapshot();
        await _contextUpdater.UpdateAsync(snapshot, token).ConfigureAwait(false);
    }

    // TODO: Maybe it's worth to extract to some base class
    private void EnsureCircuitBreakerIsHalfOpen(CircuitBreakerContext context)
    {
        if (!CanHandle(context))
            throw new InvalidCircuitBreakerStateException(context.Name, context.State,
                CircuitBreakerState.HalfOpen);
    }
}