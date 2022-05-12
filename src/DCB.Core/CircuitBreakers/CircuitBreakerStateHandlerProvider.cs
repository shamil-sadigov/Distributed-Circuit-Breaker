using DCB.Core.CircuitBreakers.States;

namespace DCB.Core.CircuitBreakers;

// TODO: Register in IoC


// Instead of creating it, it's better to inject it
internal sealed class CircuitBreakerStateHandlerProvider
{
    private Dictionary<CircuitBreakerStateEnum, Func<ICircuitBreakerStateHandler>> stateMapper = new();

    internal CircuitBreakerStateHandlerProvider()
    {
        stateMapper[CircuitBreakerStateEnum.Open] = () => new OpenCircuitBreakerStateHandler();
        stateMapper[CircuitBreakerStateEnum.HalfOpen] = () => new HalfOpenCircuitBreakerStateHandler();
        stateMapper[CircuitBreakerStateEnum.Closed] = () => new ClosedCircuitBreakerStateHandler();
    }
    
    // TODO: Write test to ensure that StateMapper maps all necessary enums

    internal ICircuitBreakerStateHandler Map(CircuitBreakerStateEnum stateNum)
    {
        if (stateMapper.TryGetValue(stateNum, out var stateFactory))
            return stateFactory();

        throw new InvalidOperationException($"No state is provided for {stateNum}");
    }
}