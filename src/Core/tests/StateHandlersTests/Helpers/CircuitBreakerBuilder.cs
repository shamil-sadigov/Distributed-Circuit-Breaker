using Core.Context;
using Core.Settings;
using FluentAssertions.Extensions;

namespace Core.Tests.StateHandlersTests.Helpers;

public sealed class CircuitBreakerBuilder
{
    private CircuitBreakerSnapshot _snapshot;
    
    private readonly CircuitBreakerSettings? _settings;
    private ISystemClock? _systemClock;
    private readonly SystemClockStub _defaultSystemClock = new();

    private CircuitBreakerBuilder(CircuitBreakerSnapshot snapshot, CircuitBreakerSettings circuitBreakerSettings)
    {
        _snapshot = snapshot;
        _settings = circuitBreakerSettings;
    }

    public static CircuitBreakerBuilder ClosedCircuitBreakerWith(CircuitBreakerSettings settings)
    {
        var snapshot = new CircuitBreakerSnapshot("Default", 0, null);
        return new CircuitBreakerBuilder(snapshot, settings);
    }
    
    public static CircuitBreakerBuilder OpenCircuitBreakerWith(CircuitBreakerSettings settings)
    {
        var snapshot = new CircuitBreakerSnapshot(
            Name: "Default", 
            FailedTimes: settings.FailureAllowed + 1, 
            DateTime.UtcNow);
        
        return new CircuitBreakerBuilder(snapshot, settings);
    }

    public static CircuitBreakerBuilder HalfOpenCircuitBreakerWith(CircuitBreakerSettings settings)
    {
        var snapshot = new CircuitBreakerSnapshot(
            "Default", 
            FailedTimes: settings.FailureAllowed, 
            LastTimeFailed: DateTime.UtcNow - settings.DurationOfBreak - 1.Seconds());
        
        return new CircuitBreakerBuilder(snapshot, settings);
    }

  
    public CircuitBreakerBuilder WithFailedTimes(int failedCount, DateTime? lastTimeFailedAt = null)
    {
        _snapshot = _snapshot with
        {
            FailedTimes = failedCount,
            LastTimeFailed = lastTimeFailedAt ?? _defaultSystemClock.CurrentUtcTime
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
        return CircuitBreakerContext.BuildFromState(
            _snapshot, 
            _settings ?? throw new InvalidOperationException("_settings was not set"), 
            _systemClock ?? _defaultSystemClock);
    }
}