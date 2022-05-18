using DCB.Core.CircuitBreakers.Context;
using FluentAssertions.Extensions;

namespace DCB.Core.Tests.StateHandlers.Helpers;

public class CircuitBreakerBuilder
{
    private CircuitBreakerContextSnapshot _snapshot;
    private ISystemClock? _systemClock;

    public CircuitBreakerBuilder(CircuitBreakerContextSnapshot snapshot)
    {
        _snapshot = snapshot;
    }

    public static CircuitBreakerBuilder BuildClosedCircuitBreaker()
    {
        var closedCircuitBreaker = new CircuitBreakerContextSnapshot("Default", 3, 0, true, null, null, 5.Seconds());
        return new CircuitBreakerBuilder(closedCircuitBreaker);
    }


    public static CircuitBreakerBuilder BuildOpenCircuitBreaker()
    {
        var closedCircuitBreaker = new CircuitBreakerContextSnapshot(
            "Default", 3, 3, false, DateTime.UtcNow + 5.Seconds(), DateTime.UtcNow, 5.Seconds());
        return new CircuitBreakerBuilder(closedCircuitBreaker);
    }

    public static CircuitBreakerBuilder BuildHalfOpenCircuitBreaker()
    {
        var closedCircuitBreaker = new CircuitBreakerContextSnapshot(
            "Default", 3, 3, false, DateTime.UtcNow - 5.Seconds(), DateTime.UtcNow - 10.Seconds(), 5.Seconds());
        return new CircuitBreakerBuilder(closedCircuitBreaker);
    }

    public CircuitBreakerBuilder WithFailedCount(int failedCount)
    {
        _snapshot = _snapshot with
        {
            FailedCount = failedCount
        };
        return this;
    }

    public CircuitBreakerBuilder WillTransitToHalfOpenStateAt(DateTime transitionToHalfOpenStateAt)
    {
        _snapshot = _snapshot with
        {
            TransitionDateToHalfOpenState = transitionToHalfOpenStateAt
        };
        return this;
    }

    public CircuitBreakerBuilder LastTimeStateChangedAt(DateTime lastTimeStateChangedAt)
    {
        _snapshot = _snapshot with
        {
            LastTimeStateChanged = lastTimeStateChangedAt
        };
        return this;
    }

    public CircuitBreakerBuilder WithAllowedNumberOfFailures(int allowedFailures)
    {
        _snapshot = _snapshot with
        {
            FailureAllowedBeforeBreaking = allowedFailures
        };
        return this;
    }

    public CircuitBreakerBuilder WithDurationOfBreak(TimeSpan durationOfBreak)
    {
        _snapshot = _snapshot with
        {
            DurationOfBreak = durationOfBreak
        };

        return this;
    }

    public CircuitBreakerBuilder UsingSystemClock(ISystemClock systemClock)
    {
        _systemClock = systemClock;
        return this;
    }

    public CircuitBreakerContext Build()
    {
        var currentTime = _systemClock?.CurrentTime ?? DateTime.UtcNow;
        return CircuitBreakerContext.BuildFromSnapshot(_snapshot, currentTime);
    }

    public static implicit operator CircuitBreakerContext(CircuitBreakerBuilder builder)
    {
        return builder.Build();
    }
}