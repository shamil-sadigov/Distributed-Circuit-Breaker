using DCB.Core.CircuitBreakers.Context;
using DCB.Core.CircuitBreakers.StateHandlers;

namespace DCB.Core.CircuitBreakers;

// TODO: Register in IoC

// Instead of creating it, it's better to inject it
public sealed class CircuitBreakerStateHandlerProvider
{
    private readonly IEnumerable<ICircuitBreakerStateHandler> _stateHandlers;

    public CircuitBreakerStateHandlerProvider(IEnumerable<ICircuitBreakerStateHandler> stateHandlers)
    {
        _stateHandlers = stateHandlers;
    }

    // TODO: Write test to ensure that StateMapper maps all necessary enums

    internal ICircuitBreakerStateHandler GetHandler(CircuitBreakerContext context)
    {
        // TODO: What to do if there is more than one handler ?

        var stateHandler = _stateHandlers.SingleOrDefault(x => x.CanHandle(context));

        if (stateHandler is null)
            throw new InvalidOperationException("No handler is found that can handle context");

        return stateHandler;
    }
}