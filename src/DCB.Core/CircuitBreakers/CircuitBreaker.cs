using DCB.Core.CircuitBreakerOption;
using DCB.Core.CircuitBreakers.Context;
using DCB.Core.CircuitBreakers.StateHandlers;
using DCB.Core.Storage;

namespace DCB.Core.CircuitBreakers;

public enum CircuitBreakerStateEnum
{
    None, Open, HalfOpen, Closed
}

internal struct Void
{
    
}

public class CircuitBreaker<TOptions> where TOptions: CircuitBreakerOptions
{
    private readonly ICircuitBreakerContextGetter _contextGetter;
    private readonly CircuitBreakerStateHandlerProvider _handlerProvider;
    public readonly TOptions CircuitBreakerOptions;
    private readonly ISystemClock _systemClock;

    // TODO: Add context property
    
    protected CircuitBreaker(
        ICircuitBreakerContextGetter contextGetter,
        CircuitBreakerStateHandlerProvider handlerProvider,
        TOptions circuitBreakerOptions,
        ISystemClock systemClock)
    {
        _contextGetter = contextGetter;
        _handlerProvider = handlerProvider;
        CircuitBreakerOptions = circuitBreakerOptions;
        _systemClock = systemClock;
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        CircuitBreakerContextSnapshot? snapshot = await _contextGetter
            .GetAsync(circuitBreakerName: CircuitBreakerOptions.Name, cancellationToken)
            .ConfigureAwait(false);
        
        CircuitBreakerContext context = BuildOrCreateCircuitBreaker<TResult>(snapshot);
        
        ICircuitBreakerStateHandler stateHandler = _handlerProvider.GetHandler(context);
        
        var result = await stateHandler.HandleAsync(action, context, CircuitBreakerOptions, cancellationToken);

        return result;
    }

    private CircuitBreakerContext BuildOrCreateCircuitBreaker<TResult>(CircuitBreakerContextSnapshot? snapshot)
    {
        CircuitBreakerContext context;
        if (snapshot is not null)
            context = CircuitBreakerContext.BuildFromSnapshot(snapshot, _systemClock.CurrentTime);
        else
            context = CircuitBreakerContext.CreateNew(
                CircuitBreakerOptions.Name,
                CircuitBreakerOptions.FailureAllowedBeforeBreaking,
                CircuitBreakerOptions.DurationOfBreak);
        return context;
    }
    
}