using DCB.Core.CircuitBreakerOption;
using DCB.Core.CircuitBreakers.States;

namespace DCB.Core.CircuitBreakers.StateHandlers;

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

            circuitBreaker.Failed(_systemClock.CurrentTime);
            await SaveAsync(circuitBreaker);
            return result;
        }
        catch (Exception ex)
        {
            if (!options.ExceptionHandlers.CanHandle(ex)) 
                throw;
            
            circuitBreaker.Failed(_systemClock.CurrentTime);
            await SaveAsync(circuitBreaker);
            throw;
        }
    }

    private async Task SaveAsync(CircuitBreakerContext circuitBreaker)
    {
        var snapshot = circuitBreaker.GetSnapshot();
        await _contextSaver.SaveAsync(snapshot).ConfigureAwait(false);
    }

    private void EnsureCircuitBreakerIsClosed(CircuitBreakerContext context)
    {
        if (!CanHandle(context))
            throw new InvalidCircuitBreakerStateException(context.Name, context.State, CircuitBreakerStateEnum.Closed);
    }

    public bool CanHandle(CircuitBreakerContext context) => context.State == CircuitBreakerStateEnum.Closed;
}