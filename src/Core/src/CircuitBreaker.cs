using Core.Context;
using Core.Settings;
using Core.StateHandlers;
using Core.Storage;

namespace Core;

public class CircuitBreaker<TOptions> : ICircuitBreaker<TOptions> where TOptions : CircuitBreakerSettings
{
    private readonly ICircuitBreakerStorage _storage;
    private readonly CircuitBreakerStateHandlerProvider _stateHandlerProvider;
    private readonly ISystemClock _systemClock;
    private readonly TOptions _circuitBreakerOptions;
    
    public CircuitBreaker(
        ICircuitBreakerStorage storage,
        CircuitBreakerStateHandlerProvider stateHandlerProvider,
        TOptions circuitBreakerOptions,
        ISystemClock systemClock)
    {
        _storage = storage;
        _stateHandlerProvider = stateHandlerProvider;
        _circuitBreakerOptions = circuitBreakerOptions;
        _systemClock = systemClock;
    }
    
    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        var context = await GetOrCreateContextAsync(cancellationToken).ConfigureAwait(false);
        
        var stateHandler = _stateHandlerProvider.GetHandler(context);

        var result = await stateHandler
            .HandleAsync(action, context, _circuitBreakerOptions, cancellationToken)
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

        var stateHandler = _stateHandlerProvider.GetHandler(context);
        
        await stateHandler
            .HandleAsync(token =>
            {
                action(token);
                return Void.Instance;
            }, context, _circuitBreakerOptions, cancellationToken)
            .ConfigureAwait(false);
    }
    
    private async Task<CircuitBreakerContext> GetOrCreateContextAsync(CancellationToken cancellationToken)
    {
        var circuitBreakerState = await _storage
            .GetAsync(_circuitBreakerOptions.Name, cancellationToken)
            .ConfigureAwait(false);
        
        if (circuitBreakerState is not null)
        {
            return CircuitBreakerContext.BuildFromState(circuitBreakerState, _circuitBreakerOptions, _systemClock);
        }
        
        // TODO: We should also consider concurrency. If two parallel requests will try to 
        // create circuitbreaker then one of them will fail, so we need to handle it and just 
        // return the one that is already exists in DB

        return await CreateNewCircuitBreakerAsync(cancellationToken).ConfigureAwait(false);
    }
    
    private async Task<CircuitBreakerContext> CreateNewCircuitBreakerAsync(CancellationToken token)
    {
        var context = CircuitBreakerContext.CreateNew(_circuitBreakerOptions, _systemClock);
        await _storage.AddAsync(context.CreateSnapshot(), token);
        return context;
    }
    
}