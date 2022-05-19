using DCB.Core.CircuitBreakerOption;
using DCB.Core.CircuitBreakers.Context;
using DCB.Core.Exceptions;
using DCB.Core.Storage;

namespace DCB.Core.CircuitBreakers.StateHandlers;

// TODO: Add logging

internal sealed class ClosedCircuitBreakerHandler : ICircuitBreakerStateHandler
{
    private readonly ICircuitBreakerContextUpdater _contextUpdater;
    private readonly ISystemClock _systemClock;

    public ClosedCircuitBreakerHandler(ICircuitBreakerContextUpdater contextUpdater, ISystemClock systemClock)
    {
        _contextUpdater = contextUpdater;
        _systemClock = systemClock;
    }

    public async Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CircuitBreakerOptions options,
        CancellationToken token)
    {
        EnsureCircuitBreakerIsClosed(circuitBreaker);

        try
        {
            var result = await action(token).ConfigureAwait(false);

            if (!options.ResultHandlers.CanHandle(result))
                return result;

            circuitBreaker.Failed(_systemClock.GetCurrentTime());
            await SaveAsync(circuitBreaker, token);
            return result;
        }
        catch (Exception ex)
        {
            if (!options.ExceptionHandlers.CanHandle(ex))
                throw;

            circuitBreaker.Failed(_systemClock.GetCurrentTime());
            await SaveAsync(circuitBreaker, token);
            throw;
        }
    }

    public bool CanHandle(CircuitBreakerContext context)
    {
        return context.State == CircuitBreakerState.Closed;
    }

    private async Task SaveAsync(CircuitBreakerContext circuitBreaker, CancellationToken token)
    {
        var snapshot = circuitBreaker.GetSnapshot();
        await _contextUpdater.UpdateAsync(snapshot, token).ConfigureAwait(false);
    }

    private void EnsureCircuitBreakerIsClosed(CircuitBreakerContext context)
    {
        if (!CanHandle(context))
            throw new InvalidCircuitBreakerStateException(context.Name, context.State, CircuitBreakerState.Closed);
    }
}