using DCB.Core.CircuitBreakerOption;

namespace DCB.Core.CircuitBreakers.States;

// TODO: Add logging


internal sealed class ClosedCircuitBreakerHandler:ICircuitBreakerStateHandler
{
    private readonly ICircuitBreakerContextSaver _contextSaver;
    private readonly ISystemClock _systemClock;

    public ClosedCircuitBreakerHandler(ICircuitBreakerContextSaver contextSaver, ISystemClock systemClock)
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

            circuitBreaker.Failed(currentTime: _systemClock.CurrentTime);
            
            await _contextSaver.SaveAsync(circuitBreaker).ConfigureAwait(false);
            return result;
        }
        catch (Exception ex)
        {
            if (!options.ExceptionHandlers.CanHandle(ex)) 
                throw;
            
            circuitBreaker.Failed(currentTime: _systemClock.CurrentTime);
            await _contextSaver.SaveAsync(circuitBreaker).ConfigureAwait(false);
            throw;
        }
    }

    private void EnsureCircuitBreakerIsClosed(CircuitBreakerContext context)
    {
        if (!CanHandle(context))
            throw new InvalidCircuitBreakerStateException(context.Name, context.State, CircuitBreakerStateEnum.Closed);
    }

    public bool CanHandle(CircuitBreakerContext context) => context.State == CircuitBreakerStateEnum.Closed;
}