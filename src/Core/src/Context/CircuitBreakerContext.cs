using Core.Policy;
using Core.Storage;
using Helpers;

namespace Core.Context;

// TODO: Add more unit tests

/// <summary>
/// Context of circuit breaker that controls state transitions
/// </summary>
public sealed partial class CircuitBreakerContext
{
    private readonly ISystemClock _clock;

    private CircuitBreakerContext(
        CircuitBreakerPolicy policy,
        int failedTimes,
        DateTime? lastTimeFailed,
        ISystemClock clock)
    {
        policy.ThrowIfNull();
        clock.ThrowIfNull();
        
        if (failedTimes < 0)
            throw new ArgumentException("Expected to be greater or equal to zero", nameof(failedTimes));

        FailedTimes = failedTimes;
        LastTimeFailed = lastTimeFailed;
        Policy = policy;
        _clock = clock;
    }

    public string Name => Policy.Name;
    public int FailedTimes { get; private set; }
    public DateTime? LastTimeFailed { get; private set; }

    private CircuitBreakerPolicy Policy { get; }

    public int FailureAllowed => Policy.FailureAllowed;

    public bool IsClosed => FailedTimes < Policy.FailureAllowed;
    public bool IsOpen => !IsClosed && !IsTimeToGiveAChance;
    public bool IsHalfOpen => !IsClosed && IsTimeToGiveAChance;

    internal CircuitBreakerState State => IsClosed
        ? CircuitBreakerState.Closed
        : IsHalfOpen
            ? CircuitBreakerState.HalfOpen
            : CircuitBreakerState.Open;


    public DateTime? TimeToTransitToHalfOpenState => IsClosed ? null : LastTimeFailed + Policy.DurationOfBreak;
    
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
        return Policy.ShouldHandleResult(result);
    }

    public bool CanHandleException(Exception exception)
    {
        return Policy.ShouldHandleException(exception);
    }
}