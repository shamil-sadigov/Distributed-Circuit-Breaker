using Core.Settings;
using Helpers;

namespace Core.Context;

// TODO: Add more unit tests
// - to test Corretly returned snapshot

public sealed partial class CircuitBreakerContext
{
    private readonly ISystemClock _clock;

    private CircuitBreakerContext(
        CircuitBreakerSettings settings,
        int failedTimes,
        DateTime? lastTimeFailed,
        ISystemClock clock)
    {
        settings.ThrowIfNull();
        clock.ThrowIfNull();
        if (failedTimes < 0)
            throw new ArgumentException("Expected to be greater or equal to zero", nameof(failedTimes));

        FailedTimes = failedTimes;
        LastTimeFailed = lastTimeFailed;
        Settings = settings;
        _clock = clock;
    }

    public string Name => Settings.Name;
    public int FailedTimes { get; private set; }
    public DateTime? LastTimeFailed { get; private set; }

    private CircuitBreakerSettings Settings { get; }

    public int FailureAllowed => Settings.FailureAllowed;

    public bool IsClosed => FailedTimes < Settings.FailureAllowed;
    public bool IsOpen => !IsClosed && !IsTimeToGiveAChance;
    public bool IsHalfOpen => !IsClosed && IsTimeToGiveAChance;

    internal CircuitBreakerState State => IsClosed
        ? CircuitBreakerState.Closed
        : IsHalfOpen
            ? CircuitBreakerState.HalfOpen
            : CircuitBreakerState.Open;


    public DateTime? TimeToTransitToHalfOpenState => IsClosed ? null : LastTimeFailed + Settings.DurationOfBreak;
    
    private bool IsTimeToGiveAChance => TimeToTransitToHalfOpenState > _clock.CurrentUtcTime;

    internal CircuitBreakerSnapshot CreateSnapshot()
    {
        return new(Name, FailedTimes, LastTimeFailed);
    }

    public void Failed()
    {
        FailedTimes++;
        LastTimeFailed = _clock.CurrentUtcTime;
    }

    public void Reset()
    {
        FailedTimes = default;
        LastTimeFailed = null;
    }

    public bool CanHandleResult<TResult>(TResult result)
    {
        return Settings.CanHandleResult(result);
    }

    public bool CanHandleException(Exception exception)
    {
        return Settings.CanHandleException(exception);
    }
}