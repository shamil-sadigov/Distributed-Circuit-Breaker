using System.Runtime.CompilerServices;
using DCB.Core.CircuitBreakerOption;



namespace DCB.Core.CircuitBreakers.States;

internal sealed class HalfOpenCircuitBreakerStateHandler:ICircuitBreakerStateHandler
{
    private readonly ISystemClock _systemClock;
    private readonly ICircuitBreakerStore _store;

    public HalfOpenCircuitBreakerStateHandler(ISystemClock systemClock, ICircuitBreakerStore store)
    {
        _systemClock = systemClock;
        _store = store;
    }
    
    
    public Task<TResult> HandleAsync<TResult>(
        CircuitBreakerOptions options,
        Func<Task<TResult>> action, CircuitBreakerContext context)
    {
        throw new NotImplementedException();
    }

    // TODO: Write unit tests to it
    public bool CanHandle(CircuitBreakerContext context)
    {
        return context.State != CircuitBreakerStateEnum.Closed 
               && context.TransitionDateToHalfOpenState >= _systemClock.UtcNow;
    }
}