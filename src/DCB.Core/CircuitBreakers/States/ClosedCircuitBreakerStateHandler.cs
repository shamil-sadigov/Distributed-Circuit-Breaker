using DCB.Core.CircuitBreakerOption;

namespace DCB.Core.CircuitBreakers.States;

// TODO: Add logging
internal sealed class ClosedCircuitBreakerStateHandler:ICircuitBreakerStateHandler
{
    private readonly ICircuitBreakerContextSaver _contextSaver;
    private readonly ISystemClock _systemClock;

    public ClosedCircuitBreakerStateHandler(ICircuitBreakerContextSaver contextSaver, ISystemClock systemClock)
    {
        _contextSaver = contextSaver;
        _systemClock = systemClock;
    }
    
    public async Task<TResult> HandleAsync<TResult>(
        CircuitBreakerOptions options,
        Func<Task<TResult>> action, 
        CircuitBreakerContext circuitBreaker)
    {
        EnsureCircuitBreakerIsClosed(circuitBreaker);
        
        try
        {
            var result = await action().ConfigureAwait(false);

            if (!options.ResultHandlers.CanHandle(result)) 
                return result;
            
            RecordFailure(circuitBreaker);
            await _contextSaver.SaveAsync(circuitBreaker).ConfigureAwait(false);
            return result;
        }
        catch (Exception ex)
        {
            if (!options.ExceptionHandlers.CanHandle(ex)) 
                throw;
            
            RecordFailure(circuitBreaker);
            await _contextSaver.SaveAsync(circuitBreaker).ConfigureAwait(false);
            throw;
        }
        
        void RecordFailure(CircuitBreakerContext breaker)
        {
            breaker.FailedCount++;

            if (breaker.FailedCount < breaker.FailureAllowedBeforeBreaking) 
                return;

            var now = _systemClock.UtcNow;

            breaker.State = CircuitBreakerStateEnum.Open;
            breaker.TransitionDateToHalfOpenState = now.Add(options.DurationOfBreak);
            breaker.LastTimeStateChanged = now;
            
        }
    }

    private void EnsureCircuitBreakerIsClosed(CircuitBreakerContext context)
    {
        if (!CanHandle(context))
            throw new InvalidCircuitBreakerStateException(context.Name, context.State, CircuitBreakerStateEnum.Closed);
    }

    public bool CanHandle(CircuitBreakerContext context) => context.State == CircuitBreakerStateEnum.Closed;
}