﻿using Core.Context;
using Core.Exceptions;
using Core.Settings;
using Core.StateHandlers;
using Core.Storage;

namespace Core;

public class CircuitBreaker<TSettings> : ICircuitBreaker<TSettings> where TSettings : CircuitBreakerSettings
{
    private readonly ICircuitBreakerStorage _circuitBreakerStorage;
    private readonly CircuitBreakerStateHandlerProvider _stateHandlerProvider;
    private readonly ISystemClock _systemClock;
    private readonly TSettings _circuitBreakerSettings;
    
    public CircuitBreaker(
        ICircuitBreakerStorage circuitBreakerStorage,
        CircuitBreakerStateHandlerProvider stateHandlerProvider,
        TSettings circuitBreakerSettings,
        ISystemClock systemClock)
    {
        _circuitBreakerStorage = circuitBreakerStorage;
        _stateHandlerProvider = stateHandlerProvider;
        _circuitBreakerSettings = circuitBreakerSettings;
        _systemClock = systemClock;
    }
    
    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        var context = await GetOrCreateContextAsync(cancellationToken).ConfigureAwait(false);
        
        var stateHandler = _stateHandlerProvider.FindHandler(context.State);

        var result = await stateHandler
            .HandleAsync(action, context, cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    public async Task<CircuitBreakerState> GetStateAsync(CancellationToken cancellationToken)
    {
        var context = await GetOrCreateContextAsync(cancellationToken).ConfigureAwait(false);
        return context.State;
    }

    public async Task ExecuteAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken)
    {
        var context = await GetOrCreateContextAsync(cancellationToken).ConfigureAwait(false);

        var stateHandler = _stateHandlerProvider.FindHandler(context.State);
        
        await stateHandler
            .HandleAsync(token =>
            {
                action(token);
                return Void.Instance;
            }, context, cancellationToken)
            .ContinueWith(async parentTask =>
            {
                if (parentTask.IsFaulted && parentTask.Exception?.InnerException is CircuitBreakerIsOpenException)
                {
                    return parentTask;   
                }

                var snapshot = context.CreateSnapshot();
                await _circuitBreakerStorage.UpdateAsync(snapshot, cancellationToken).ConfigureAwait(false);
                return parentTask;
            }, cancellationToken)
            .Unwrap()
            .Unwrap()
            .ConfigureAwait(false);
    }
    
    private async Task<CircuitBreakerContext> GetOrCreateContextAsync(CancellationToken cancellationToken)
    {
        var circuitBreakerState = await _circuitBreakerStorage
            .GetAsync(_circuitBreakerSettings.Name, cancellationToken)
            .ConfigureAwait(false);
        
        if (circuitBreakerState is not null)
        {
            return CircuitBreakerContext.BuildFromState(circuitBreakerState, _circuitBreakerSettings, _systemClock);
        }
        
        // TODO: We should also consider concurrency. If two parallel requests will try to 
        // create circuitbreaker then one of them will fail, so we need to handle it and just 
        // return the one that is already exists in DB

        return await CreateNewCircuitBreakerAsync(cancellationToken).ConfigureAwait(false);
    }
    
    private async Task<CircuitBreakerContext> CreateNewCircuitBreakerAsync(CancellationToken token)
    {
        var context = CircuitBreakerContext.CreateNew(_circuitBreakerSettings, _systemClock);
        await _circuitBreakerStorage.AddAsync(context.CreateSnapshot(), token);
        return context;
    }
}