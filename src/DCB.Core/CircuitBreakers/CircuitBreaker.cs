using DCB.Core.CircuitBreakerOption;
using DCB.Core.CircuitBreakers.Context;
using DCB.Core.Storage;

namespace DCB.Core.CircuitBreakers;

{

internal struct Void
{
}

public class CircuitBreaker<TOptions> where TOptions : CircuitBreakerOptions
{
    private readonly ICircuitBreakerContextGetter _contextGetter;
    private readonly CircuitBreakerStateHandlerProvider _handlerProvider;
    private readonly ISystemClock _systemClock;
    public readonly TOptions CircuitBreakerOptions;

    // TODO: Add context property

    protected CircuitBreaker(
        ICircuitBreakerContextGetter contextGetter,
        CircuitBreakerStateHandlerProvider handlerProvider,
        TOptions circuitBreakerOptions,
        ISystemClock systemClock)
    {
        _contextGetter = contextGetter;
        _handlerProvider = handlerProvider;
        CircuitBreakerOptions = circuitBreakerOptions;
        _systemClock = systemClock;
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        var snapshot = await _contextGetter
            .GetAsync(CircuitBreakerOptions.Name, cancellationToken)
            .ConfigureAwait(false);

        var context = BuildOrCreateCircuitBreaker<TResult>(snapshot);

        var stateHandler = _handlerProvider.GetHandler(context);

        var result = await stateHandler.HandleAsync(action, context, CircuitBreakerOptions, cancellationToken);

        return result;
    }

    private CircuitBreakerContext BuildOrCreateCircuitBreaker<TResult>(CircuitBreakerContextSnapshot? snapshot)
    {
        CircuitBreakerContext context;
        if (snapshot is not null)
            context = CircuitBreakerContext.BuildFromSnapshot(snapshot, _systemClock.CurrentTime);
        else
            context = CircuitBreakerContext.CreateNew(
                CircuitBreakerOptions.Name,
                CircuitBreakerOptions.FailureAllowedBeforeBreaking,
                CircuitBreakerOptions.DurationOfBreak);
        return context;
    }
}