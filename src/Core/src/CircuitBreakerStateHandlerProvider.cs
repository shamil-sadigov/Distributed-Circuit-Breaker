using Core.Context;
using Core.StateHandlers;

namespace Core;

public sealed class CircuitBreakerStateHandlerProvider
{
    private readonly IEnumerable<ICircuitBreakerStateHandler> _stateHandlers;

    public CircuitBreakerStateHandlerProvider(IEnumerable<ICircuitBreakerStateHandler> stateHandlers)
    {
        if (!stateHandlers.Any())
        {
            throw new ArgumentException("Should not be empty", nameof(stateHandlers));
        }

        _stateHandlers = stateHandlers;
    }

    // TODO: Write test to ensure that StateMapper maps all necessary enums

    internal ICircuitBreakerStateHandler FindHandler(CircuitBreakerState circuitBreakerState)
    {
        // TODO: What to do if there is more than one handler ?

        var stateHandler = _stateHandlers.SingleOrDefault(x => x.CanHandle(circuitBreakerState));

        if (stateHandler is null)
            throw new InvalidOperationException("No handler is found that can handle context");

        return stateHandler;
    }
}