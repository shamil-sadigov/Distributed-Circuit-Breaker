using Core.CircuitBreakerOption;
using Core.CircuitBreakers.Context;
using Core.Storage;

namespace Core.CircuitBreakers;

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

    public async Task<CircuitBreakerState> GetStateAsync(CancellationToken token)
    {
        var circuitBreakerContext = await GetOrCreateContextAsync(token).ConfigureAwait(false);
        return circuitBreakerContext.State;
    }

    public async Task<bool> IsClosedAsync(CancellationToken token)
    {
        var state = await GetStateAsync(token).ConfigureAwait(false);
        return state == CircuitBreakerState.Closed;
    }

    public async Task<bool> IsOpenAsync(CancellationToken token)
    {
        var state = await GetStateAsync(token).ConfigureAwait(false);
        return state == CircuitBreakerState.Open;
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        var context = await GetOrCreateContextAsync(cancellationToken).ConfigureAwait(false);
        
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
        var context = await GetOrCreateContextAsync(cancellationToken).ConfigureAwait(false);

        var stateHandler = _stateHandlerProvider.GetHandler(context);

        await stateHandler
            .HandleAsync(token =>
            {
                action(token);
                return _cachedVoidResult;
            }, context, _circuitBreakerOptions, cancellationToken)
            .ConfigureAwait(false);
    }
    

    private async Task<CircuitBreakerContext> GetOrCreateContextAsync(CancellationToken cancellationToken)
    {
        var snapshot = await _storage
            .GetAsync(_circuitBreakerOptions.Name, cancellationToken)
            .ConfigureAwait(false);

        CircuitBreakerContext context;

        if (snapshot is not null)
            context = CircuitBreakerContext.BuildFromSnapshot(snapshot, _systemClock.GetCurrentTime());
        else
        {
            // TODO: We should also consider concurrency. If two parallel requests will try to 
            // create circuitbreaker then one of them will fail, so we need to handle it and just 
            // return the one that is already exists in DB

            context = await CreateNewCircuitBreakerAsync(cancellationToken).ConfigureAwait(false);

        }
        
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