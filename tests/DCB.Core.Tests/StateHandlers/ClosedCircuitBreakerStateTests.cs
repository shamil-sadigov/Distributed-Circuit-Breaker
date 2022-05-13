using System.Net;
using DCB.Core.CircuitBreakers;
using DCB.Core.CircuitBreakers.States;
using DCB.Core.Tests.ResultHandler.Helpers;
using DCB.Core.Tests.StateHandlers.Helpers;
using FluentAssertions;
using FluentAssertions.Extensions;

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

        var circuitBreakerContext = CircuitBreakerContext.CreateFromData(
            new CircuitBreakerContextData()
            {
                Name = "Default",
                IsCircuitBreakerClosed = true,
                FailedCount = currentFailedCount,
                FailureAllowedBeforeBreaking = failureAllowedBeforeBreaking,
                DurationOfBreak = 5.Seconds()
            }, clock.CurrentTime);

        var sut = new ClosedCircuitBreakerStateHandler(saverSpy, clock);
        
        // Act
        await sut.Invoking(x => x.HandleAsync<CustomResult>(
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
        
        var clock = new SystemClockStub();
        clock.SetUtcDate(DateTime.UtcNow);
        
        var options = new TestCircuitBreakerOptions();
        options.HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable);
        
        var circuitBreakerContext = CircuitBreakerContext.CreateFromData(
            new CircuitBreakerContextData()
            {
                Name = "Default",
                IsCircuitBreakerClosed = true,
                FailedCount = currentFailedCount,
                FailureAllowedBeforeBreaking = failureAllowedBeforeBreaking,
                DurationOfBreak = 5.Seconds()
            }, clock.CurrentTime);
        
        var sut = new ClosedCircuitBreakerStateHandler(saverSpy, clock);
        
        // Act & Assert
        await sut.Invoking(x => x.HandleAsync<CustomResult>(
                options,
                () => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable),
                circuitBreakerContext))
            .Should()
            .ThrowAsync<CustomHttpException>();
        
        var savedCircuitBreaker = saverSpy.SavedCircuitBreaker;

        var expectedTransitionDateToHalfOpenState = clock.CurrentTime + options.DurationOfBreak;

        savedCircuitBreaker.Should().NotBeNull();
        savedCircuitBreaker!.ShouldBeOpenWithFailedCount(expectedFailedCount);
        savedCircuitBreaker!.LastTimeStateChanged.Should().Be(clock.CurrentTime);
        savedCircuitBreaker!.TransitionDateToHalfOpenState.Should().Be(expectedTransitionDateToHalfOpenState);
    }
    
    [Fact]
    public async Task CircuitBreaker_is_not_changed_if_no_exception_handling_specified()
    {
        // Arrange
        var saverSpy = new CircuitBreakerSaverSpy();
        var clock = new SystemClockStub();
        var options = new TestCircuitBreakerOptions();
        
        var circuitBreakerContext = CircuitBreakerContext.CreateFromData(
            new CircuitBreakerContextData()
            {
                Name = "Default",
                IsCircuitBreakerClosed = true,
                FailedCount = 1,
                FailureAllowedBeforeBreaking = 3,
                DurationOfBreak = 5.Seconds()
            }, clock.CurrentTime);
        
        var sut = new ClosedCircuitBreakerStateHandler(saverSpy, clock);
        
        // Act
        await sut.Invoking(x => x.HandleAsync<CustomResult>(
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
        
        var circuitBreakerContext = CircuitBreakerContext.CreateFromData(
            new CircuitBreakerContextData()
            {
                Name = "Default",
                IsCircuitBreakerClosed = false,
                FailedCount = 3,
                FailureAllowedBeforeBreaking = 3,
                DurationOfBreak = 5.Minutes(),
                TransitionDateToHalfOpenState = clock.CurrentTime + 5.Minutes(),
                LastTimeStateChanged = clock.CurrentTime - 10.Seconds()
            }, clock.CurrentTime);
        
        var sut = new ClosedCircuitBreakerStateHandler(saverSpy, clock);
        
        // Act
        sut.Invoking(x => x.HandleAsync(
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