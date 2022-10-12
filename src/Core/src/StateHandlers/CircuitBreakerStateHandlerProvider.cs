using Core.Context;
using Microsoft.Extensions.Logging;

namespace Core.StateHandlers;

public sealed class CircuitBreakerStateHandlerProvider
{
    private readonly IEnumerable<ICircuitBreakerStateHandler> _stateHandlers;
    private readonly ILoggerFactory _loggerFactory;

    public CircuitBreakerStateHandlerProvider(
        IEnumerable<ICircuitBreakerStateHandler> stateHandlers,
        ILoggerFactory loggerFactory)
    {
        if (!stateHandlers.Any())
        {
            throw new ArgumentException("Should not be empty", nameof(stateHandlers));
        }

        _stateHandlers = stateHandlers;
        _loggerFactory = loggerFactory;
    }


    internal ICircuitBreakerStateHandler FindHandler(CircuitBreakerState circuitBreakerState)
    {
        // TODO: What to do if there is more than one handler ?

        // It's O(n), but for now it's not harmful as we only have 3 handlers
        // but still need to refactor
        var stateHandler = _stateHandlers.SingleOrDefault(x => x.CanHandle(circuitBreakerState));

        if (stateHandler is null)
            throw new InvalidOperationException($"No handler is found that can handle state '{circuitBreakerState}'");
        
        return WithDecorator(stateHandler);
    }

    private ICircuitBreakerStateHandler WithDecorator(ICircuitBreakerStateHandler stateHandler)
    {
        return new CircuitBreakerStateHandlerLoggingDecorator(
            stateHandler,
            _loggerFactory.CreateLogger<CircuitBreakerStateHandlerLoggingDecorator>());
    }
}