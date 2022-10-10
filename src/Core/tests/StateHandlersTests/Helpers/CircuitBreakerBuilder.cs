using Core.CircuitBreakers.Context;
using FluentAssertions.Extensions;

namespace Core.Tests.StateHandlersTests.Helpers;

public class CircuitBreakerBuilder
{
    private CircuitBreakerState _state;
    private ISystemClock? _systemClock;

    public CircuitBreakerBuilder(CircuitBreakerState state)
    {
        _state = state;
    }

    public static CircuitBreakerBuilder BuildClosedCircuitBreaker()
    {
        var closedCircuitBreaker = new CircuitBreakerState("Default", 3, 0, true, null, null, 5.Seconds());
        return new CircuitBreakerBuilder(closedCircuitBreaker);
    }


    public static CircuitBreakerBuilder BuildOpenCircuitBreaker()
    {
        var closedCircuitBreaker = new CircuitBreakerState(
            "Default", 3, 3, false, DateTime.UtcNow + 5.Seconds(), DateTime.UtcNow, 5.Seconds());
        return new CircuitBreakerBuilder(closedCircuitBreaker);
    }

    public static CircuitBreakerBuilder BuildHalfOpenCircuitBreaker()
    {
        var closedCircuitBreaker = new CircuitBreakerState(
            "Default", 3, 3, false, DateTime.UtcNow - 5.Seconds(), DateTime.UtcNow - 10.Seconds(), 5.Seconds());
        return new CircuitBreakerBuilder(closedCircuitBreaker);
    }

    public CircuitBreakerBuilder WithFailedCount(int failedCount)
    {
        _state = _state with
        {
            FailedCount = failedCount
        };
        return this;
    }

    public CircuitBreakerBuilder WillTransitToHalfOpenStateAt(DateTime transitionToHalfOpenStateAt)
    {
        _state = _state with
        {
            TransitionDateToHalfOpenState = transitionToHalfOpenStateAt
        };
        return this;
    }

    public CircuitBreakerBuilder LastTimeStateChangedAt(DateTime lastTimeStateChangedAt)
    {
        _state = _state with
        {
            LastTimeFailed = lastTimeStateChangedAt
        };
        return this;
    }

    public CircuitBreakerBuilder WithAllowedNumberOfFailures(int allowedFailures)
    {
        _state = _state with
        {
            FailureAllowedBeforeBreaking = allowedFailures
        };
        return this;
    }

    public CircuitBreakerBuilder WithDurationOfBreak(TimeSpan durationOfBreak)
    {
        _state = _state with
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
        var currentTime = _systemClock?.GetCurrentUtcTime() ?? DateTime.UtcNow;
        return CircuitBreakerContext.Build(_state, currentTime);
    }

    public static implicit operator CircuitBreakerContext(CircuitBreakerBuilder builder)
    {
        return builder.Build();
    }
}