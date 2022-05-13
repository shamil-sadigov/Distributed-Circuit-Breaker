using DCB.Core.CircuitBreakerOption;

namespace DCB.Core.CircuitBreakers.States;


// TODO: Register in IoC
public interface ISystemClock
{
    DateTime CurrentTime { get; }
}

internal sealed class OpenCircuitBreakerStateHandler:ICircuitBreakerStateHandler
{
    private readonly ISystemClock _systemClock;

    public OpenCircuitBreakerStateHandler(ISystemClock systemClock)
    {
        _systemClock = systemClock;
    }
    
    public Task<TResult> HandleAsync<TResult>(
        CircuitBreakerOptions options,
        Func<Task<TResult>> action, 
        CircuitBreakerContext circuitBreaker)
    {

        EnsureCircuitBreakerIsOpen(circuitBreaker);
        
        // TODO: More descriptive Exception message is needed 
        throw new CircuitBreakerIsOpenException($"Circuit breaker with name '{circuitBreaker.Name}' is open");
    }

    public bool CanHandle(CircuitBreakerContext context)
    {
        return context.State != CircuitBreakerStateEnum.Closed 
            && context.TransitionDateToHalfOpenState <= _systemClock.CurrentTime;
    }
    
    private void EnsureCircuitBreakerIsOpen(CircuitBreakerContext context)
    {
        if (!CanHandle(context))
            throw new InvalidCircuitBreakerStateException(context.Name, context.State, CircuitBreakerStateEnum.Open);
    }
}