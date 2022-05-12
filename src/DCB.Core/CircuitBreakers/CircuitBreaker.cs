using DCB.Core.CircuitBreakerOption;
using DCB.Core.CircuitBreakers.States;

namespace DCB.Core.CircuitBreakers;

public enum CircuitBreakerStateEnum
{
    None, Open, HalfOpen, Closed
}

public class CircuitBreaker<TOptions> where TOptions: CircuitBreakerOptions
{
    private readonly ICircuitBreakerContextGetter _contextGetter;
    private readonly CircuitBreakerStateHandlerProvider _handlerProvider;
    public readonly TOptions Options;

    // TODO: Add context property
    
    protected CircuitBreaker(
        ICircuitBreakerContextGetter contextGetter,
        CircuitBreakerStateHandlerProvider handlerProvider,
        TOptions options)
    {
        _contextGetter = contextGetter;
        _handlerProvider = handlerProvider;
        Options = options;
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        CircuitBreakerContext stateEnum = await _contextGetter.GetAsync(circuitBreakerName: Options.Name);

        ICircuitBreakerStateHandler stateHandler = _handlerProvider.GetHandler(stateEnum);

        // return stateHandler.HandleAsync<TResult>(Options, Store, TODO, TODO);

        throw new NotImplementedException();
    }
}