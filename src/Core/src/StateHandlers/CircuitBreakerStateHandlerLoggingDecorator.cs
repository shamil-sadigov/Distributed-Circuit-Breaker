using Core.Context;
using Core.Logging;
using Microsoft.Extensions.Logging;

namespace Core.StateHandlers;

// TODO: Register in IoC
public class CircuitBreakerStateHandlerLoggingDecorator:ICircuitBreakerStateHandler
{
    private readonly ICircuitBreakerStateHandler _decoratee;
    private readonly ILogger<CircuitBreakerStateHandlerLoggingDecorator> _logger;

    public CircuitBreakerStateHandlerLoggingDecorator(
        ICircuitBreakerStateHandler decoratee,
        ILogger<CircuitBreakerStateHandlerLoggingDecorator> logger)
    {
        _decoratee = decoratee;
        _logger = logger;
    }
    
    public async Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CircuitBreakerContext circuitBreaker, CancellationToken token)
    {
        _logger.LogCircuitBreakerState(circuitBreaker);

        var result =  await _decoratee.HandleAsync(action, circuitBreaker, token);
        
        _logger.LogCircuitBreakerState(circuitBreaker);

        return result;
    }

    public bool CanHandle(CircuitBreakerState state)
    {
        return _decoratee.CanHandle(state);
    }
}