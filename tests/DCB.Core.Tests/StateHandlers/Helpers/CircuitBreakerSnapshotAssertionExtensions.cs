using DCB.Core.CircuitBreakers.Context;
using FluentAssertions;

namespace DCB.Core.Tests.StateHandlers.Helpers;

public class CircuitBreakerSnapshotAssertion
{
    private readonly CircuitBreakerContextSnapshot _context;

    public CircuitBreakerSnapshotAssertion(CircuitBreakerContextSnapshot context)
    {
        _context = context;
    }

    public CircuitBreakerSnapshotAssertion Closed()
    {
        _context.IsCircuitBreakerClosed.Should().BeTrue();
        return this;
    }
    
    public CircuitBreakerSnapshotAssertion NotClosed()
    {
        _context.IsCircuitBreakerClosed.Should().BeFalse();
        return this;
    }

    public CircuitBreakerSnapshotAssertion WithFailedCount(int failedCount)
    {
        _context.FailedCount.Should().Be(failedCount);
        return this;
    }

    public CircuitBreakerSnapshotAssertion LastTimeStateChangedAt(DateTime stateChangedAt)
    {
        _context.LastTimeStateChanged.Should().Be(stateChangedAt);
        return this;
    }
    
    public CircuitBreakerSnapshotAssertion WillTransitToHalfOpenState(DateTime stateChangedAt)
    {
        _context.TransitionDateToHalfOpenState.Should().Be(stateChangedAt);
        return this;
    }
    
    public CircuitBreakerSnapshotAssertion WillNotTransitToHalfOpenState()
    {
        _context.TransitionDateToHalfOpenState.Should().Be(null);
        return this;
    }
}


public static class CircuitBreakerSnapshotAssertionExtensions
{
    public static CircuitBreakerSnapshotAssertion ShouldBe(this CircuitBreakerContextSnapshot snapshot)
    {
        return new CircuitBreakerSnapshotAssertion(snapshot);
    } 
}