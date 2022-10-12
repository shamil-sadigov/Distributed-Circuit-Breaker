using Core.Context;
using Core.StateHandlers;
using Microsoft.Extensions.Logging;

namespace Core;

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

    // TODO: Write test to ensure that StateMapper maps all necessary enums

    internal ICircuitBreakerStateHandler FindHandler(CircuitBreakerState circuitBreakerState)
    {
        // TODO: What to do if there is more than one handler ?

        // It's O(n), but for now it's not harmful as with only have 3 handlers
        // but still need to refactor
        var stateHandler = _stateHandlers.SingleOrDefault(x => x.CanHandle(circuitBreakerState));

        if (stateHandler is null)
            throw new InvalidOperationException("No handler is found that can handle context");
        
        return InDecorator(stateHandler);
    }

    private ICircuitBreakerStateHandler InDecorator(ICircuitBreakerStateHandler stateHandler)
    {
        return new CircuitBreakerStateHandlerLoggingDecorator(
            stateHandler,
            _loggerFactory.CreateLogger<CircuitBreakerStateHandlerLoggingDecorator>());
    }
}