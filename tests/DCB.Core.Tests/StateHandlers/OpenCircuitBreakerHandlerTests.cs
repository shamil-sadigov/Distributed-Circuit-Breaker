using System.Net;
using DCB.Core.CircuitBreakers;
using DCB.Core.CircuitBreakers.States;
using DCB.Core.Tests.ResultHandler.Helpers;
using DCB.Core.Tests.StateHandlers.Helpers;
using FluentAssertions;
using FluentAssertions.Extensions;

namespace DCB.Core.Tests.StateHandlers;

public class OpenCircuitBreakerStateTests
{
    [Theory]
    [InlineData(2, 0)]
    [InlineData(3, 2)]
    [InlineData(4, 1)]
    public void Cannot_handle_closed_circuit_breaker(
        int failureAllowedBeforeBreaking,
        int currentFailedCount)
    {
        // Arrange
        var clock = new SystemClockStub();
        
        var circuitBreaker = CircuitBreakerContext.CreateFromData(
            new CircuitBreakerContextData()
            {
                Name = "Default",
                IsCircuitBreakerClosed = true,
                FailedCount = currentFailedCount,
                FailureAllowedBeforeBreaking = failureAllowedBeforeBreaking,
                DurationOfBreak = 5.Minutes(),
                LastTimeStateChanged = clock.CurrentTime - 10.Seconds()
            }, clock.CurrentTime);
        
        var sut = new OpenCircuitBreakerHandler();

        // Act & Assert

        circuitBreaker.State.Should().Be(CircuitBreakerStateEnum.Closed);

        sut.CanHandle(circuitBreaker)
            .Should().BeFalse();
    }
    
    [Theory]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    public void Cannot_handle_half_open_circuit_breaker(
        int failureAllowedBeforeBreaking,
        int currentFailedCount)
    {
        // Arrange
        var clock = new SystemClockStub();
        
        var circuitBreaker = CircuitBreakerContext.CreateFromData(
            new CircuitBreakerContextData()
            {
                Name = "Default",
                IsCircuitBreakerClosed = false,
                FailedCount = currentFailedCount,
                FailureAllowedBeforeBreaking = failureAllowedBeforeBreaking,
                DurationOfBreak = 5.Minutes(),
                LastTimeStateChanged = clock.CurrentTime - 30.Seconds(),
                TransitionDateToHalfOpenState = clock.CurrentTime - 10.Seconds(),
            }, clock.CurrentTime);
        
        var sut = new OpenCircuitBreakerHandler();

        // Act & Assert

        circuitBreaker.State.Should().Be(CircuitBreakerStateEnum.HalfOpen);

        sut.CanHandle(circuitBreaker)
            .Should().BeFalse();
    }
    
    [Theory]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    public void Can_handle_open_circuit_breaker(
        int failureAllowedBeforeBreaking,
        int currentFailedCount)
    {
        // Arrange
        var clock = new SystemClockStub();
        
        var circuitBreaker = CircuitBreakerContext.CreateFromData(
            new CircuitBreakerContextData()
            {
                Name = "Default",
                IsCircuitBreakerClosed = false,
                FailedCount = currentFailedCount,
                FailureAllowedBeforeBreaking = failureAllowedBeforeBreaking,
                DurationOfBreak = 5.Minutes(),
                LastTimeStateChanged = clock.CurrentTime - 30.Seconds(),
                TransitionDateToHalfOpenState = clock.CurrentTime + 5.Seconds(),
            }, clock.CurrentTime);
        
        var sut = new OpenCircuitBreakerHandler();

        // Act & Assert

        circuitBreaker.State.Should().Be(CircuitBreakerStateEnum.Open);

        sut.CanHandle(circuitBreaker)
            .Should().BeTrue();
    }
    
    
    [Theory]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    [InlineData(4, 4)]
    public async Task When_circuit_breaker_is_open_then_action_is_impossible_to_invoke(
        int failureAllowedBeforeBreaking,
        int currentFailedCount)
    {
        // Arrange
        var clock = new SystemClockStub();
        var options = new TestCircuitBreakerOptions();

        options.HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable)
               .HandleResult<CustomResult>(x => !x.IsSuccessful);
        
        var circuitBreakerContext = CircuitBreakerContext.CreateFromData(
            new CircuitBreakerContextData()
            {
                Name = "Default",
                IsCircuitBreakerClosed = false,
                FailedCount = currentFailedCount,
                FailureAllowedBeforeBreaking = failureAllowedBeforeBreaking,
                DurationOfBreak = 5.Minutes(),
                TransitionDateToHalfOpenState = clock.CurrentTime + 5.Minutes(),
                LastTimeStateChanged = clock.CurrentTime - 10.Seconds()
            }, clock.CurrentTime);
        
        var sut = new OpenCircuitBreakerHandler();

        // Act & Assert
        await sut.Invoking(x => x.HandleAsync(
                            options, () => Task.FromResult(new CustomResult(IsSuccessful: true)),
                            circuitBreakerContext))
            .Should()
            .ThrowAsync<CircuitBreakerIsOpenException>();
        
    }
}