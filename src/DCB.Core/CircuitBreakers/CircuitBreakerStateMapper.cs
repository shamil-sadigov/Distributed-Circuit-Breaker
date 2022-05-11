using DCB.Core.CircuitBreakers.States;

namespace DCB.Core.CircuitBreakers;

// TODO: Register in IoC
internal sealed class CircuitBreakerStateMapper
{
    private Dictionary<CircuitBreakerStateEnum, Func<ICircuitBreakerState>> stateMapper = new();

    internal CircuitBreakerStateMapper()
    {
        stateMapper[CircuitBreakerStateEnum.Open] = () => new OpenCircuitBreakerState();
        stateMapper[CircuitBreakerStateEnum.HalfOpen] = () => new HalfOpenCircuitBreakerState();
        stateMapper[CircuitBreakerStateEnum.Closed] = () => new ClosedCircuitBreakerState();
    }
    
    // TODO: Write test to ensure that StateMapper maps all necessary enums

    internal ICircuitBreakerState Map(CircuitBreakerStateEnum stateNum)
    {
        if (stateMapper.TryGetValue(stateNum, out var stateFactory))
            return stateFactory();

        throw new InvalidOperationException($"No state is provided for {stateNum}");
    }
}