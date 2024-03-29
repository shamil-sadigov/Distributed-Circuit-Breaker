﻿using Core.Context;
using Core.Helpers;

namespace Core.StateHandlers;

// TODO: Add logging
internal sealed class HalfOpenCircuitBreakerStateHandler : ICircuitBreakerStateHandler
{
    public async Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CancellationToken token)
    {
        circuitBreaker.EnsureStateIs(CircuitBreakerState.HalfOpen);
        
        token.ThrowIfCancellationRequested();

        
        try
        {
            var result = await action(token).ConfigureAwait(false);

            if (circuitBreaker.CanHandleResult(result))
                circuitBreaker.Failed();
            else
                circuitBreaker.Reset();

            return result;
        }
        catch (Exception ex)
        {
            if (circuitBreaker.CanHandleException(ex))
                circuitBreaker.Failed();
            
            throw;
        }
    }

    public bool CanHandle(CircuitBreakerState state) => state is CircuitBreakerState.HalfOpen;
}