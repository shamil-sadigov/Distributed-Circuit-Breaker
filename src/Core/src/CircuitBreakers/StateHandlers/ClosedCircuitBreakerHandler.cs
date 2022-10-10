﻿using Core.CircuitBreakerOption;
using Core.CircuitBreakers.Context;
using Core.Exceptions;
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
        CircuitBreakerOptions options,
        CancellationToken token)
    {
        try
        {
            var result = await action(token).ConfigureAwait(false);

            if (!options.CanHandleResult(result))
                return result;

            circuitBreaker.Failed();
            await SaveAsync(circuitBreaker.State, token);
            return result;
        }
        catch (Exception ex)
        {
            if (!options.CanHandleException(ex))
                throw;

            circuitBreaker.Failed();
            await SaveAsync(circuitBreaker.State, token);
            throw;
        }
    }

    public bool CanHandle(CircuitBreakerContext context)
    {
        return context.IsClosed();
    }

    private async Task SaveAsync(CircuitBreakerState circuitBreakerState, CancellationToken token)
    {
        await _contextUpdater.UpdateAsync(circuitBreakerState, token).ConfigureAwait(false);
    }
    
}