using DCB.Core.CircuitBreakerOption;
using DCB.Core.Exceptions;
using DCB.Core.Storage;

namespace DCB.Core.CircuitBreakers.StateHandlers;

// TODO: Add logging
internal sealed class HalfOpenCircuitBreakerStateHandler:ICircuitBreakerStateHandler
{
    private readonly ISystemClock _systemClock;
    private readonly ICircuitBreakerContextUpdater _contextUpdater;

    public HalfOpenCircuitBreakerStateHandler(
        ISystemClock systemClock,
        ICircuitBreakerContextUpdater contextUpdater)
    {
        _systemClock = systemClock;
        _contextUpdater = contextUpdater;
    }
    
    public async Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action,
        CircuitBreakerContext.CircuitBreakerContext circuitBreaker,
        CircuitBreakerOptions options, CancellationToken token)
    {
        EnsureCircuitBreakerIsHalfOpen(circuitBreaker);
        
        try
        {
            var result = await action(token).ConfigureAwait(false);

            if (!options.ResultHandlers.CanHandle(result))
            {
                circuitBreaker.Close(_systemClock.CurrentTime);
                await SaveAsync(circuitBreaker, token);
                return result;
            }
            
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

    private async Task SaveAsync(CircuitBreakerContext.CircuitBreakerContext circuitBreaker, CancellationToken token)
    {
        var snapshot = circuitBreaker.GetSnapshot();
        await _contextUpdater.UpdateAsync(snapshot, token).ConfigureAwait(false);
    }
    
    public bool CanHandle(CircuitBreakerContext.CircuitBreakerContext context) 
        => context.State == CircuitBreakerStateEnum.HalfOpen;
    
    // TODO: Maybe it's worth to extract to some base class
    private void EnsureCircuitBreakerIsHalfOpen(CircuitBreakerContext.CircuitBreakerContext context)
    {
        if (!CanHandle(context))
            throw new InvalidCircuitBreakerStateException(context.Name, context.State, CircuitBreakerStateEnum.HalfOpen);
    }
}