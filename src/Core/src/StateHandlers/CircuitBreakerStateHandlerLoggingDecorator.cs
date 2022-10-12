using Core.Context;
using Core.Helpers;
using Core.Logging;
using Microsoft.Extensions.Logging;

namespace Core.StateHandlers;

// TODO: poor solution, but it's temporarily
[SkipAutoWiring]
public sealed class CircuitBreakerStateHandlerLoggingDecorator:ICircuitBreakerStateHandler
{
    private readonly ICircuitBreakerStateHandler _decoratedHandler;
    private readonly ILogger<CircuitBreakerStateHandlerLoggingDecorator> _logger;

    public CircuitBreakerStateHandlerLoggingDecorator(
        ICircuitBreakerStateHandler decoratedHandler,
        ILogger<CircuitBreakerStateHandlerLoggingDecorator> logger)
    {
        _decoratedHandler = decoratedHandler;
        _logger = logger;
    }
    
    public async Task<TResult> HandleAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CircuitBreakerContext circuitBreaker, CancellationToken token)
    {
        _logger.LogCircuitBreakerState(circuitBreaker);

        var result =  await _decoratedHandler.HandleAsync(action, circuitBreaker, token);
        
        _logger.LogCircuitBreakerState(circuitBreaker);

        return result;
    }

    public bool CanHandle(CircuitBreakerState state)
    {
        return _decoratedHandler.CanHandle(state);
    }
}