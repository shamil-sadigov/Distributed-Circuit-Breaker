using Core.Policy;

namespace Core.Tests.StateHandlersTests.Helpers;

public sealed class CircuitBreakerBuilder
{
    private CircuitBreakerSnapshot _snapshot;
    
    private readonly CircuitBreakerPolicy? _policy;
    private ISystemClock? _systemClock;
    private readonly SystemClockStub _defaultSystemClock = new();

    private CircuitBreakerBuilder(CircuitBreakerSnapshot snapshot, CircuitBreakerPolicy circuitBreakerPolicy)
    {
        _snapshot = snapshot;
        _policy = circuitBreakerPolicy;
    }

    public static CircuitBreakerBuilder ClosedCircuitBreakerWith(CircuitBreakerPolicy policy)
    {
        var snapshot = new CircuitBreakerSnapshot("Default", 0, null);
        return new CircuitBreakerBuilder(snapshot, policy);
    }
    
    public static CircuitBreakerBuilder OpenCircuitBreakerWith(CircuitBreakerPolicy policy)
    {
        var snapshot = new CircuitBreakerSnapshot(
            Name: "Default", 
            FailedTimes: policy.FailureAllowed + 1, 
            DateTime.UtcNow);
        
        return new CircuitBreakerBuilder(snapshot, policy);
    }

    public static CircuitBreakerBuilder HalfOpenCircuitBreakerWith(CircuitBreakerPolicy policy)
    {
        var snapshot = new CircuitBreakerSnapshot(
            "Default", 
            FailedTimes: policy.FailureAllowed, 
            LastTimeFailed: DateTime.UtcNow - policy.DurationOfBreak - 1.Seconds());
        
        return new CircuitBreakerBuilder(snapshot, policy);
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
            _policy ?? throw new InvalidOperationException("_policy was not set"), 
            _systemClock ?? _defaultSystemClock);
    }
}