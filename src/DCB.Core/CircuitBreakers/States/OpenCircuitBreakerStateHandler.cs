using DCB.Core.CircuitBreakerOption;

namespace DCB.Core.CircuitBreakers.States;


// TODO: Register in IoC
internal interface ISystemClock
{
    DateTime UtcNow { get; }
}


internal sealed class OpenCircuitBreakerStateHandler:ICircuitBreakerStateHandler
{
    private readonly ISystemClock _systemClock;
    private readonly ICircuitBreakerStore _store;

    public OpenCircuitBreakerStateHandler(ISystemClock systemClock, ICircuitBreakerStore store)
    {
        _systemClock = systemClock;
        _store = store;
    }
    
    public Task<TResult> HandleAsync<TResult>(
        CircuitBreakerOptions options,
        Func<Task<TResult>> action, 
        CircuitBreakerContext context)
    {
        // TODO: Should throw CircuitBreakerIsOpenException();
        
        throw new NotImplementedException();
    }

    // TODO: Write unit tests to it
    public bool CanHandle(CircuitBreakerContext context)
    {
        return context.State != CircuitBreakerStateEnum.Closed 
            && context.TransitionDateToHalfOpenState <= _systemClock.UtcNow;
    }
}