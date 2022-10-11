using Core.CircuitBreakerOption;
using Helpers;

namespace Core.CircuitBreakers.Context;

// TODO: Add more unit tests

public enum CircuitBreakerState
{
    Closed,
    Open,
    HalfOpen
}

public sealed partial class CircuitBreakerContext
{
    private readonly ISystemClock _clock;

    private CircuitBreakerContext(
        ICircuitBreakerSettings settings,
        int failedCount,
        DateTime? lastTimeFailed,
        ISystemClock clock)
    {
        settings.ThrowIfNull();
        clock.ThrowIfNull();
        if (failedCount < 0)
            throw new ArgumentException("Expected to be greater or equal to zero", nameof(failedCount));

        FailedCount = failedCount;
        LastTimeFailed = lastTimeFailed;
        Settings = settings;
        _clock = clock;
    }

    internal string Name => Settings.Name;
    private int FailedCount { get; set; }
    private DateTime? LastTimeFailed { get; set; }

    private ICircuitBreakerSettings Settings { get; }

    internal CircuitBreakerSnapshot CreateSnapshot() 
        => new(Name, FailedCount, LastTimeFailed);
    
    private bool IsClosed => FailedCount < Settings.FailureAllowedBeforeBreaking;
    private bool IsHalfOpen => !IsClosed && IsTimeToGiveAChance;

    internal CircuitBreakerState State => 
        IsClosed ? CircuitBreakerState.Closed 
        : IsHalfOpen ? CircuitBreakerState.HalfOpen
        : CircuitBreakerState.Open;
    
    private bool IsTimeToGiveAChance => LastTimeFailed + Settings.DurationOfBreak > _clock.UtcTime;
    
    internal void Failed()
    {
        FailedCount++;
        LastTimeFailed = _clock.UtcTime;
    }

    internal void Reset()
    {
        FailedCount = default;
        LastTimeFailed = null;
    }
}