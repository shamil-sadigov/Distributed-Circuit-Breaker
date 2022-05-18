using DCB.Core.CircuitBreakerOption;
using DCB.Core.CircuitBreakers.States;

namespace DCB.Core.CircuitBreakers.StateHandlers;

// TODO: Add logging
internal sealed class HalfOpenCircuitBreakerStateHandler:ICircuitBreakerStateHandler
{
    private readonly ISystemClock _systemClock;
    private readonly ICircuitBreakerContextSaver _contextSaver;

    public HalfOpenCircuitBreakerStateHandler(
        ISystemClock systemClock,
        ICircuitBreakerContextSaver contextSaver)
    {
        _systemClock = systemClock;
        _contextSaver = contextSaver;
    }
    
    public async Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext circuitBreaker,
        CircuitBreakerOptions options, CancellationToken token)
    {
        EnsureCircuitBreakerIsHalfOpen(circuitBreaker);
        
        try
        {
            var result = await action(token).ConfigureAwait(false);

            if (!options.ResultHandlers.CanHandle(result))
            {
                circuitBreaker.Close(_systemClock.CurrentTime);
                await SaveAsync(circuitBreaker);
                return result;
            }
            
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
    
    public bool CanHandle(CircuitBreakerContext context) 
        => context.State == CircuitBreakerStateEnum.HalfOpen;
    
    // TODO: Maybe it's worth to extract to some base class
    private void EnsureCircuitBreakerIsHalfOpen(CircuitBreakerContext context)
    {
        if (!CanHandle(context))
            throw new InvalidCircuitBreakerStateException(context.Name, context.State, CircuitBreakerStateEnum.HalfOpen);
    }
}