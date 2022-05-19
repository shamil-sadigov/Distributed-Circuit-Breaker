using DCB.Core.CircuitBreakerOption;
using DCB.Core.CircuitBreakers.Context;
using DCB.Core.Exceptions;
using DCB.Core.Storage;

namespace DCB.Core.CircuitBreakers;

public class CircuitBreaker<TOptions> : ICircuitBreaker<TOptions> where TOptions : CircuitBreakerOptions
{
    private readonly ICircuitBreakerStorage _storage;
    private readonly CircuitBreakerStateHandlerProvider _stateHandlerProvider;
    private readonly ISystemClock _systemClock;
    private readonly TOptions _circuitBreakerOptions;
    private readonly Task<Void> _cachedVoidResult = Task.FromResult(new Void());
    
    public CircuitBreaker(
        ICircuitBreakerStorage storage,
        CircuitBreakerStateHandlerProvider stateHandlerProvider,
        TOptions circuitBreakerOptions,
        ISystemClock systemClock)
    {
        _storage = storage;
        _stateHandlerProvider = stateHandlerProvider;
        _circuitBreakerOptions = circuitBreakerOptions;
        _systemClock = systemClock;
    }

    public async Task<CircuitBreakerState> GetStateAsync()
    {
        var snapshot = await _storage
            .GetAsync(_circuitBreakerOptions.Name, CancellationToken.None)
            .ConfigureAwait(false);
        
        if (snapshot is null)
            throw new CircuitBreakerSnapshotNotFoundException(_circuitBreakerOptions.Name);

        var circuitBreakerContext = CircuitBreakerContext.BuildFromSnapshot(snapshot, _systemClock.GetCurrentTime());

        return circuitBreakerContext.State;
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        var context = await GetContextAsync(cancellationToken).ConfigureAwait(false);
        
        var stateHandler = _stateHandlerProvider.GetHandler(context);

        var result = await stateHandler
            .HandleAsync(action, context, _circuitBreakerOptions, cancellationToken)
            .ConfigureAwait(false);

        return result;
    }
    
    public async Task ExecuteAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken)
    {
        var context = await GetContextAsync(cancellationToken).ConfigureAwait(false);

        var stateHandler = _stateHandlerProvider.GetHandler(context);

        await stateHandler
            .HandleAsync(token =>
            {
                action(token);
                return _cachedVoidResult;
            }, context, _circuitBreakerOptions, cancellationToken)
            .ConfigureAwait(false);
    }
    

    private async Task<CircuitBreakerContext> GetContextAsync(CancellationToken cancellationToken)
    {
        var snapshot = await _storage
            .GetAsync(_circuitBreakerOptions.Name, cancellationToken)
            .ConfigureAwait(false);

        CircuitBreakerContext context;

        if (snapshot is not null)
            context = CircuitBreakerContext.BuildFromSnapshot(snapshot, _systemClock.GetCurrentTime());
        else
            context = await CreateNewCircuitBreakerAsync(cancellationToken).ConfigureAwait(false);
        return context;
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