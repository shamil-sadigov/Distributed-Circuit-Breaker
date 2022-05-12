using DCB.Core.CircuitBreakers;
using DCB.Core.CircuitBreakers.States;
using FluentAssertions;

namespace DCB.Core.Tests;

public static class CircuitBreakerContextTestExtensions
{
    public static CircuitBreakerContext ShouldBeClosedWithFailedCount(this CircuitBreakerContext circuitBreaker, int expectedFailedCount)
    {
        circuitBreaker.FailedCount.Should().Be(expectedFailedCount);
        circuitBreaker.State.Should().Be(CircuitBreakerStateEnum.Closed);
        circuitBreaker.TransitionDateToHalfOpenState.Should().BeNull();
        
        return circuitBreaker;
    }
    
    public static CircuitBreakerContext ShouldBeOpenWithFailedCount(
        this CircuitBreakerContext circuitBreaker,
        int expectedFailedCount)
    {
        circuitBreaker.FailedCount.Should().Be(expectedFailedCount);
        circuitBreaker.State.Should().Be(CircuitBreakerStateEnum.Open);

        return circuitBreaker;
    }

    
}