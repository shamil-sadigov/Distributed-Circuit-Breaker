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
    
    public async Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CircuitBreakerOptions options,
        CancellationToken token)
    {
        EnsureCircuitBreakerIsClosed(circuitBreaker);
        
        try
        {
            var result = await action(token).ConfigureAwait(false);

            if (!options.ResultHandlers.CanHandle(result)) 
                return result;

            circuitBreaker.Failed(_systemClock.CurrentTime);
            await SaveAsync(circuitBreaker, token);
            return result;
        }
        catch (Exception ex)
        {
            if (!options.ExceptionHandlers.CanHandle(ex)) 
                throw;
            
            circuitBreaker.Failed(_systemClock.CurrentTime);
            await SaveAsync(circuitBreaker, token);
            throw;
        }
    }

    private async Task SaveAsync(CircuitBreakerContext circuitBreaker, CancellationToken token)
    {
        var snapshot = circuitBreaker.GetSnapshot();
        await _contextSaver.SaveAsync(snapshot, token).ConfigureAwait(false);
    }

    private void EnsureCircuitBreakerIsClosed(CircuitBreakerContext context)
    {
        if (!CanHandle(context))
            throw new InvalidCircuitBreakerStateException(context.Name, context.State, CircuitBreakerStateEnum.Closed);
    }

    public bool CanHandle(CircuitBreakerContext context) => context.State == CircuitBreakerStateEnum.Closed;
}