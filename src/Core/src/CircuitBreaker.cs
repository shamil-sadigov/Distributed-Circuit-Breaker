using Core.Context;
using Core.Logging;
using Core.Settings;
using Core.Storage;
using Microsoft.Extensions.Logging;

namespace Core;

// TODO: Write tests to check that context is saved
public class CircuitBreaker<TSettings> : ICircuitBreaker<TSettings> where TSettings : CircuitBreakerSettings
{
    private readonly ICircuitBreakerStorage _circuitBreakerStorage;
    private readonly CircuitBreakerStateHandlerProvider _stateHandlerProvider;
    private readonly ISystemClock _systemClock;
    private readonly ILogger<CircuitBreaker<TSettings>> _logger;
    private readonly TSettings _circuitBreakerSettings;
    
    public CircuitBreaker(
        ICircuitBreakerStorage circuitBreakerStorage,
        CircuitBreakerStateHandlerProvider stateHandlerProvider,
        TSettings circuitBreakerSettings,
        ISystemClock systemClock,
        ILogger<CircuitBreaker<TSettings>> logger)
    {
        _circuitBreakerStorage = circuitBreakerStorage;
        _stateHandlerProvider = stateHandlerProvider;
        _circuitBreakerSettings = circuitBreakerSettings;
        _systemClock = systemClock;
        _logger = logger;
    }
    
    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        var context = await GetOrCreateContextAsync(cancellationToken).ConfigureAwait(false);
        
        var stateHandler = _stateHandlerProvider.FindHandler(context.State);

        return await stateHandler
            .HandleAsync(action, context, cancellationToken)
            .ContinueWithSavingContext(context, _circuitBreakerStorage, cancellationToken)
            .Unwrap()
            .ConfigureAwait(false);
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
            .HandleAsync(async token =>
            {
                await action(token);
                return Void.Instance;
            }, context, cancellationToken)
            .Unwrap()
            .ContinueWithSavingContext(context, _circuitBreakerStorage, cancellationToken)
            .Unwrap()
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Circuit breaker context will be crated only once, when first execution occured
    /// </summary>
    private async Task<CircuitBreakerContext> GetOrCreateContextAsync(CancellationToken token)
    {
        var circuitBreakerState = await _circuitBreakerStorage
            .GetAsync(_circuitBreakerSettings.Name, token)
            .ConfigureAwait(false);
        
        if (circuitBreakerState is not null)
        {
            return CircuitBreakerContext.BuildFromState(circuitBreakerState, _circuitBreakerSettings, _systemClock);
        }
        
        // In concurrent environment, if two parellel requests reached this point
        // it's not harmful, because saving the same 'context' is idempotent, calm down...
        
        var context = CircuitBreakerContext.CreateNew(_circuitBreakerSettings, _systemClock);
        await _circuitBreakerStorage.SaveAsync(context.CreateSnapshot(), token);
        
        _logger.LogCircuitBreakerContextCreated(context);
        return context;
    }
}