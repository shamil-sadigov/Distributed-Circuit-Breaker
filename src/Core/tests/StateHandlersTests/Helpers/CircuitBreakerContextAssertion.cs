using Core.Context;
using FluentAssertions;

namespace Core.Tests.StateHandlersTests.Helpers;

public class CircuitBreakerContextAssertion
{
    private readonly CircuitBreakerContext _context;

    public CircuitBreakerContextAssertion(CircuitBreakerContext context)
    {
        _context = context;
    }

    public CircuitBreakerContextAssertion Closed()
    {
        _context.State.Should().Be(CircuitBreakerState.Closed);
        return this;
    }

    public CircuitBreakerContextAssertion Open()
    {
        _context.State.Should().Be(CircuitBreakerState.Open);
        return this;
    }

    public CircuitBreakerContextAssertion WithFailedTimes(int failedCount)
    {
        _context.FailedTimes.Should().Be(failedCount);
        return this;
    }

    public CircuitBreakerContextAssertion LastTimeFailedAt(DateTime stateChangedAt)
    {
        _context.LastTimeFailed.Should().Be(stateChangedAt);
        return this;
    }

    public CircuitBreakerContextAssertion WillTransitToHalfOpenStateAt(DateTime transitToHalfOpenStateDate)
    {
        _context.TimeToTransitToHalfOpenState.Should().Be(transitToHalfOpenStateDate);
        return this;
    }
}