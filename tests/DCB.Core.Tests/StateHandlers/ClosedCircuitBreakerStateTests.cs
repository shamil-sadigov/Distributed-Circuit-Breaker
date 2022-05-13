using System.Net;
using DCB.Core.CircuitBreakers;
using DCB.Core.CircuitBreakers.States;
using DCB.Core.Tests.ResultHandler.Helpers;
using DCB.Core.Tests.StateHandlers.Helpers;
using FluentAssertions;

namespace DCB.Core.Tests.StateHandlers;

public class ClosedCircuitBreakerStateTests
{
    
    // TODO: come up with better test name
    [Theory]
    [InlineData(4, 0, 1)]
    [InlineData(4, 1, 2)]
    [InlineData(4, 2, 3)]
    public async Task CircuitBreaker_failed_count_is_incremented_in_case_of_failure(
        int failureAllowedBeforeBreaking,
        int currentFailedCount,
        int expectedFailedCount)
    {
        // Arrange
        var saverSpy = new CircuitBreakerSaverSpy();
        var clock = new SystemClockStub();
        var options = new TestCircuitBreakerOptions();
        
        options.HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable);
        
        var circuitBreakerContext = new CircuitBreakerContext()
        {
            State = CircuitBreakerStateEnum.Closed,
            FailedCount = currentFailedCount,
            FailureAllowedBeforeBreaking = failureAllowedBeforeBreaking
        };
        
        var closedStateHandler = new ClosedCircuitBreakerStateHandler(saverSpy, clock);
        
        // Act
        await closedStateHandler
            .Invoking(x => x.HandleAsync<CustomResult>(
                            options,
                            () => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable),
                            circuitBreakerContext))
            .Should()
            .ThrowAsync<CustomHttpException>();
        
        // Assert
        var savedCircuitBreaker = saverSpy.SavedCircuitBreaker;

        savedCircuitBreaker.Should().NotBeNull();
        savedCircuitBreaker!.ShouldBeClosedWithFailedCount(expectedFailedCount);
        savedCircuitBreaker!.LastTimeStateChanged.Should().BeNull();
    }
    

    [Theory]
    [InlineData(3, 2, 3)]
    [InlineData(4, 3, 4)]
    [InlineData(5, 4, 5)]
    public async Task CircuitBreaker_is_opened_when_allowed_failure_count_is_exceeded(
        int failureAllowedBeforeBreaking,
        int currentFailedCount,
        int expectedFailedCount)
    {
        // Arrange
        var saverSpy = new CircuitBreakerSaverSpy();

        var now = DateTime.UtcNow;

        var clock = new SystemClockStub();
        
        clock.SetUtcDate(now);
        
        var options = new TestCircuitBreakerOptions();
        
        options.HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable);
        
        var circuitBreakerContext = new CircuitBreakerContext()
        {
            State = CircuitBreakerStateEnum.Closed,
            FailedCount = currentFailedCount,
            FailureAllowedBeforeBreaking = failureAllowedBeforeBreaking
        };
        
        var closedStateHandler = new ClosedCircuitBreakerStateHandler(saverSpy, clock);
        
        // Act
        await closedStateHandler
            .Invoking(x => x.HandleAsync<CustomResult>(
                options,
                () => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable),
                circuitBreakerContext))
            .Should()
            .ThrowAsync<CustomHttpException>();
        
        // Assert
        var savedCircuitBreaker = saverSpy.SavedCircuitBreaker;

        var expectedTransitionDateToHalfOpenState = now + options.DurationOfBreak;

        savedCircuitBreaker.Should().NotBeNull();
        savedCircuitBreaker!.ShouldBeOpenWithFailedCount(expectedFailedCount);
        savedCircuitBreaker!.LastTimeStateChanged.Should().Be(now);
        savedCircuitBreaker!.TransitionDateToHalfOpenState.Should().Be(expectedTransitionDateToHalfOpenState);
    }
    
    [Fact]
    public async Task CircuitBreaker_is_not_changed_if_no_exception_handling_specified()
    {
        // Arrange
        var saverSpy = new CircuitBreakerSaverSpy();
        var clock = new SystemClockStub();
        var options = new TestCircuitBreakerOptions();
        
        var circuitBreakerContext = new CircuitBreakerContext()
        {
            State = CircuitBreakerStateEnum.Closed,
            FailedCount = 1,
            FailureAllowedBeforeBreaking = 3
        };
        
        var closedStateHandler = new ClosedCircuitBreakerStateHandler(saverSpy, clock);
        
        // Act
        await closedStateHandler
            .Invoking(x => x.HandleAsync<CustomResult>(
                options,
                () => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable),
                circuitBreakerContext))
            .Should()
            .ThrowAsync<CustomHttpException>();
        
        // Assert
        var savedCircuitBreaker = saverSpy.SavedCircuitBreaker;

        savedCircuitBreaker.Should().BeNull();
    }
    
    
    [Fact]
    public void Cannot_handle_circuit_breaker_which_is_not_closed()
    {
        // Arrange
        var saverSpy = new CircuitBreakerSaverSpy();
        var clock = new SystemClockStub();
        var options = new TestCircuitBreakerOptions();
        
        var circuitBreakerContext = new CircuitBreakerContext()
        {
            State = CircuitBreakerStateEnum.Open,
            FailedCount = 3,
            FailureAllowedBeforeBreaking = 3
        };
        
        var closedStateHandler = new ClosedCircuitBreakerStateHandler(saverSpy, clock);
        
        // Act
        closedStateHandler.Invoking(x => x.HandleAsync(
                options,
                () => Task.FromResult(new CustomResult(IsSuccessful: true)),
                circuitBreakerContext))
            .Should()
            .ThrowAsync<InvalidCircuitBreakerStateException>()
            .Result
            .Which.Should().Match<InvalidCircuitBreakerStateException>
                (ex => ex.ActualState == CircuitBreakerStateEnum.Open &&
                       ex.ExpectedState == CircuitBreakerStateEnum.Closed &&
                       ex.CircuitBreakerName == circuitBreakerContext.Name);

        // Assert
        var savedCircuitBreaker = saverSpy.SavedCircuitBreaker;

        savedCircuitBreaker.Should().BeNull();
    }
}