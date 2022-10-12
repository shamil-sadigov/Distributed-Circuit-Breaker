using System.Net;
using Core.Context;
using Core.Exceptions;
using Core.StateHandlers;
using Core.Tests.ResultHandlerTests.Helpers;
using Core.Tests.StateHandlersTests.Helpers;
using FluentAssertions;
using FluentAssertions.Extensions;
using static Core.Tests.StateHandlersTests.Helpers.CircuitBreakerBuilder;

namespace Core.Tests.StateHandlersTests;

public class ClosedCircuitBreakerStateTests
{
    [Fact]
    public void Should_be_able_to_handle_circuit_breaker_only_in_open_state()
    {
        var sut = new ClosedCircuitBreakerHandler();

        foreach (var circuitBreakerState in Enum.GetValues<CircuitBreakerState>())
            sut.CanHandle(circuitBreakerState)
                .Should()
                .Be(circuitBreakerState is CircuitBreakerState.Closed);
    }
    
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
        var settings = new TestCircuitBreakerSettings()
        {
            FailureAllowed = failureAllowedBeforeBreaking,
            DurationOfBreak = 5.Seconds()
        };

        settings.HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable);

        var circuitBreakerContext = ClosedCircuitBreakerWith(settings)
            .WithFailedTimes(currentFailedCount)
            .Build();

        var sut = new ClosedCircuitBreakerHandler();

        // Act % Assert
        await sut.Invoking(x => x.HandleAsync<CustomResult>(
                _ => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable),
                circuitBreakerContext,
                CancellationToken.None))
            .Should()
            .ThrowAsync<CustomHttpException>();
        
        circuitBreakerContext!.State
            .Should().Be(CircuitBreakerState.Closed);

        circuitBreakerContext.FailedTimes.Should().Be(expectedFailedCount);
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
        var clock = new SystemClockStub();
        clock.SetUtcDate(DateTime.UtcNow);

        var settings = new TestCircuitBreakerSettings()
        {
            FailureAllowed = failureAllowedBeforeBreaking,
            DurationOfBreak = 5.Seconds()
        };
        
        settings.HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable);

        var circuitBreakerContext = ClosedCircuitBreakerWith(settings)
            .WithFailedTimes(currentFailedCount)
            .UsingSystemClock(clock)
            .Build();

        var sut = new ClosedCircuitBreakerHandler();

        // Act & Assert
        await sut.Invoking(x => x.HandleAsync<CustomResult>(
                _ => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable),
                circuitBreakerContext,
                CancellationToken.None))
            .Should()
            .ThrowAsync<CustomHttpException>();
        
        var transitToHalfOpenStateDate = clock.CurrentUtcTime + settings.DurationOfBreak;

        circuitBreakerContext!.ShouldBe()
            .Open()
            .WithFailedTimes(expectedFailedCount)
            .LastTimeFailedAt(clock.CurrentUtcTime)
            .WillTransitToHalfOpenStateAt(transitToHalfOpenStateDate);
    }

    [Fact]
    public async Task CircuitBreaker_is_not_changed_if_no_exception_handling_specified()
    {
        // Arrange
        var clock = new SystemClockStub();
        clock.SetUtcDate(DateTime.UtcNow);
        
        var settings = new TestCircuitBreakerSettings()
        {
            FailureAllowed = 3,
            DurationOfBreak = 5.Seconds()
        };

        var circuitBreakerContext = ClosedCircuitBreakerWith(settings)
            .WithFailedTimes(1, lastTimeFailedAt: clock.CurrentUtcTime)
            .UsingSystemClock(clock)
            .Build();

        var sut = new ClosedCircuitBreakerHandler();

        // Act & Assert
        await sut.Invoking(x => x.HandleAsync<CustomResult>(
                _ => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable),
                circuitBreakerContext,
                CancellationToken.None))
            .Should()
            .ThrowAsync<CustomHttpException>();
        
        circuitBreakerContext.ShouldBe()
            .Closed()
            .WithFailedTimes(1)
            .LastTimeFailedAt(clock.CurrentUtcTime);    
    }


    [Fact]
    public void Cannot_handle_circuit_breaker_which_is_not_closed()
    {
        // Arrange
        var settings = new TestCircuitBreakerSettings()
        {
            FailureAllowed = 2,
            DurationOfBreak = 20.Seconds()
        };

        CircuitBreakerContext circuitBreakerContext = OpenCircuitBreakerWith(settings)
            .Build();

        var sut = new ClosedCircuitBreakerHandler();

        // Act
        sut.Invoking(x => x.HandleAsync(_ => Task.FromResult(new CustomResult(true)),
                circuitBreakerContext,
                CancellationToken.None))
            .Should()
            .ThrowAsync<InvalidCircuitBreakerStateException>()
            .Result
            .Which.Message.Should().Match("Expected circuit breaker in 'Closed' st*");
    }
}