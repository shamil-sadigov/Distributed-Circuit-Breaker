using System.Net;
using Core.Context;
using Core.StateHandlers;
using Core.Tests.ResultHandlerTests.Helpers;
using Core.Tests.StateHandlersTests.Helpers;
using FluentAssertions;
using FluentAssertions.Extensions;
using static Core.Tests.StateHandlersTests.Helpers.CircuitBreakerBuilder;

namespace Core.Tests.StateHandlersTests;

public class HalfOpenCircuitBreakerHandlerTests
{
    [Fact]
    public void Can_handle_circuit_breaker_only_in_halfOpen_state()
    {
        var sut = new ClosedCircuitBreakerHandler();

        foreach (var circuitBreakerState in Enum.GetValues<CircuitBreakerState>())
            if (circuitBreakerState is CircuitBreakerState.HalfOpen)
                sut.CanHandle(circuitBreakerState).Should().BeTrue();
            else
                sut.CanHandle(circuitBreakerState).Should().BeFalse();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    public async Task HalfOpenCircuitBreaker_is_opened_in_case_of_failure(int failedTimes)
    {
        // Arrange
        var clock = new SystemClockStub();
        clock.SetUtcDate(DateTime.UtcNow);

        var settings = new TestCircuitBreakerSettings()
        {
            FailureAllowed = failedTimes,
            DurationOfBreak = 5.Seconds()
        };
        
        settings.HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable);

        var circuitBreakerContext = HalfOpenCircuitBreakerWith(settings)
            .UsingSystemClock(clock)
            .Build();

        var sut = new HalfOpenCircuitBreakerStateHandler();

        // Act & Assert
        await sut.Invoking(x => x.HandleAsync<CustomResult>(
                _ => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable),
                circuitBreakerContext, CancellationToken.None))
            .Should()
            .ThrowAsync<CustomHttpException>();
        
        circuitBreakerContext!.ShouldBe()
            .Open()
            .WithFailedTimes(++failedTimes)
            .LastTimeFailedAt(clock.CurrentUtcTime);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    public async Task HalfOpenCircuitBreaker_remain_half_open_when_exception_is_not_handled(int failedCount)
    {
        // Arrange
        var saverSpy = new CircuitBreakerUpdaterSpy();
        var clock = new SystemClockStub();
        var options = new TestCircuitBreakerSettings();

        clock.SetUtcDate(DateTime.UtcNow);

        var circuitBreakerContext = HalfOpenCircuitBreakerWith()
            .WithFailedTimes(failedCount)
            .WithAllowedNumberOfFailures(failedCount)
            .UsingSystemClock(clock);

        var sut = new HalfOpenCircuitBreakerStateHandler(clock, saverSpy);

        // Act
        await sut.Invoking(x => x.HandleAsync<CustomResult>(_ => throw new InvalidOperationException(),
                circuitBreakerContext, options, CancellationToken.None))
            .Should()
            .ThrowAsync<InvalidOperationException>();

        // Assert
        var savedCircuitBreaker = saverSpy.UpdatedCircuitBreaker;

        savedCircuitBreaker.Should().BeNull();
    }

    [Fact]
    public async Task CircuitBreaker_is_closed_when_no_failure_happened()
    {
        // Arrange
        var saverSpy = new CircuitBreakerUpdaterSpy();
        var clock = new SystemClockStub();
        var options = new TestCircuitBreakerSettings();

        options.HandleResult<CustomResult>(x => !x.IsSuccessful);

        clock.SetUtcDate(DateTime.UtcNow);

        var circuitBreakerContext = HalfOpenCircuitBreakerWith()
            .UsingSystemClock(clock);

        var sut = new HalfOpenCircuitBreakerStateHandler(clock, saverSpy);

        // Act
        await sut.HandleAsync(_ => Task.FromResult(new CustomResult(true)),
            circuitBreakerContext, options, CancellationToken.None);

        // Assert
        var savedCircuitBreaker = saverSpy.UpdatedCircuitBreaker;

        savedCircuitBreaker.Should().NotBeNull();

        savedCircuitBreaker!.ShouldBe()
            .Closed()
            .LastTimeFailedAt(clock.CurrentUtcTime())
            .WillNotTransitToHalfOpenState()
            .WithFailedCount(0);
    }
}