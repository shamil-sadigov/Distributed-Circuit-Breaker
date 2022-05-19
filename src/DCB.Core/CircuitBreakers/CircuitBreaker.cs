using DCB.Core.CircuitBreakerOption;
using DCB.Core.CircuitBreakers.Context;
using DCB.Core.Exceptions;
using DCB.Core.Storage;

namespace DCB.Core.CircuitBreakers;

public class CircuitBreaker<TOptions> : ICircuitBreaker<TOptions> where TOptions : CircuitBreakerOptions
{
    private readonly ICircuitBreakerStorage _storage;
    private readonly CircuitBreakerStateHandlerProvider _handlerProvider;
    private readonly ISystemClock _systemClock;
    private readonly TOptions _circuitBreakerOptions;
    
    public CircuitBreaker(
        ICircuitBreakerStorage storage,
        CircuitBreakerStateHandlerProvider handlerProvider,
        TOptions circuitBreakerOptions,
        ISystemClock systemClock)
    {
        _storage = storage;
        _handlerProvider = handlerProvider;
        _circuitBreakerOptions = circuitBreakerOptions;
        _systemClock = systemClock;
    }

    public async Task<CircuitBreakerState> GetStateAsync()
    {
        var snapshot = await _storage.GetAsync(_circuitBreakerOptions.Name, CancellationToken.None);
        
        if (snapshot is null)
            throw new CircuitBreakerSnapshotNotFoundException(_circuitBreakerOptions.Name);

        var circuitBreakerContext = CircuitBreakerContext.BuildFromSnapshot(snapshot, _systemClock.GetCurrentTime());

        return circuitBreakerContext.State;
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        var snapshot = await _storage
            .GetAsync(_circuitBreakerOptions.Name, cancellationToken)
            .ConfigureAwait(false);
        
        CircuitBreakerContext context;
        
        if (snapshot is not null)
            context = CircuitBreakerContext.BuildFromSnapshot(snapshot, _systemClock.GetCurrentTime());
        else
            context = await CreateNewCircuitBreakerAsync(cancellationToken);

        var stateHandler = _handlerProvider.GetHandler(context);

        var result = await stateHandler.HandleAsync(action, context, _circuitBreakerOptions, cancellationToken);

        return result;
    }

    private async Task<CircuitBreakerContext> CreateNewCircuitBreakerAsync(CancellationToken token)
    {
        var context = CircuitBreakerContext.CreateNew(
                _circuitBreakerOptions.Name,
                _circuitBreakerOptions.FailureAllowedBeforeBreaking,
                _circuitBreakerOptions.DurationOfBreak);

        var snapshot = context.GetSnapshot();
        await _storage.AddAsync(snapshot, token);
        return context;
    }
}