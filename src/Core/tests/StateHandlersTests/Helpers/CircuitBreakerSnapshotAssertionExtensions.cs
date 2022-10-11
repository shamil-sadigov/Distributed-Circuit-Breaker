using FluentAssertions;

namespace Core.Tests.StateHandlersTests.Helpers;

public class CircuitBreakerSnapshotAssertion
{
    private readonly CircuitBreakerSnapshot _context;

    public CircuitBreakerSnapshotAssertion(CircuitBreakerSnapshot context)
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
        _context.LastTimeFailed.Should().Be(stateChangedAt);
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
    public static CircuitBreakerSnapshotAssertion ShouldBe(this CircuitBreakerSnapshot snapshot)
    {
        return new CircuitBreakerSnapshotAssertion(snapshot);
    }
}