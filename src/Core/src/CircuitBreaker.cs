using Core.Context;
using Core.Logging;
using Core.Settings;
using Core.Storage;
using Helpers;
using Microsoft.Extensions.Logging;

namespace Core;

public class CircuitBreaker<TSettings> : ICircuitBreaker<TSettings> where TSettings : CircuitBreakerSettings
{
    private readonly ICircuitBreakerStorage _circuitBreakerStorage;
    private readonly ILogger<CircuitBreaker<TSettings>> _logger;
    private readonly CircuitBreakerStateHandlerProvider _stateHandlerProvider;
    private readonly ISystemClock _systemClock;

    public CircuitBreaker(
        ICircuitBreakerStorage circuitBreakerStorage,
        CircuitBreakerStateHandlerProvider stateHandlerProvider,
        TSettings circuitBreakerSettings,
        ISystemClock systemClock,
        ILogger<CircuitBreaker<TSettings>> logger)
    {
        _circuitBreakerStorage = circuitBreakerStorage.ThrowIfNull();
        _stateHandlerProvider = stateHandlerProvider.ThrowIfNull();
        _systemClock = systemClock.ThrowIfNull();
        _logger = logger.ThrowIfNull();
        Settings = circuitBreakerSettings.ThrowIfNull();
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken)
    {
        var context = await GetOrCreateContextAsync(cancellationToken).ConfigureAwait(false);

        var stateHandler = _stateHandlerProvider.FindHandler(context.State);

        return await stateHandler
            .HandleAsync(action, context, cancellationToken)
            .ContinueWithSavingContext(context, _circuitBreakerStorage, cancellationToken)
            .Unwrap()
            .ConfigureAwait(false);
    }

    public TSettings Settings { get; }

    public async Task<CircuitBreakerState> GetStateAsync(CancellationToken cancellationToken)
    {
        var context = await GetOrCreateContextAsync(cancellationToken).ConfigureAwait(false);
        return context.State;
    }

    public async Task ExecuteAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken)
    {
        var context = await GetOrCreateContextAsync(cancellationToken).ConfigureAwait(false);

        var stateHandler = _stateHandlerProvider.FindHandler(context.State);

        await stateHandler
            .HandleAsync(async token =>
            {
                await action(token);
                return Void.Instance;
            }, context, cancellationToken)
            .Unwrap()
            .ContinueWithSavingContext(context, _circuitBreakerStorage, cancellationToken)
            .Unwrap()
            .ConfigureAwait(false);
    }

    /// <summary>
    ///     Circuit breaker context will be created only once, when first action is executed
    /// </summary>
    private async Task<CircuitBreakerContext> GetOrCreateContextAsync(CancellationToken token)
    {
        var circuitBreakerState = await _circuitBreakerStorage
            .GetAsync(Settings.Name, token)
            .ConfigureAwait(false);

        if (circuitBreakerState is not null)
            return CircuitBreakerContext.BuildFromState(circuitBreakerState, Settings, _systemClock);

        // In concurrent environment, if two parellel requests reached this point
        // it's not harmful, because saving the same 'context' is idempotent...

        var context = CircuitBreakerContext.CreateNew(Settings, _systemClock);
        await _circuitBreakerStorage.SaveAsync(context.CreateSnapshot(), token);

        _logger.LogCircuitBreakerContextCreated(context);
        return context;
    }
}