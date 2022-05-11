using DCB.Core.CircuitBreakerOption;
using DCB.Core.CircuitBreakers.States;

namespace DCB.Core.CircuitBreakers;

public enum CircuitBreakerStateEnum
{
    None, Open, HalfOpen, Closed
}

public class CircuitBreaker<TOptions> where TOptions: CircuitBreakerOptions
{
    internal readonly ICircuitBreakerStore Store;
    public readonly TOptions Options;
    private readonly CircuitBreakerStateMapper _stateMapper = new();

    // TODO: Add context property
    
    protected CircuitBreaker(
        ICircuitBreakerStore store,
        TOptions options)
    {
        Store = store;
        Options = options;
    }

    public Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        var stateEnum = Store.GetState(circuitBreakerName: Options.Name);

        ICircuitBreakerState state = _stateMapper.Map(stateEnum);

        return state.ExecuteAsync<TResult>(Options, Store);
    }
}