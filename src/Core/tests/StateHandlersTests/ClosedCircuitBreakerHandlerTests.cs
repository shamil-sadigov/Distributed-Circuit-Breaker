using System.Net;
using Core.Context;
using Core.Exceptions;
using Core.StateHandlers;
using Core.Storage;
using Core.Tests.ResultHandlerTests.Helpers;
using Core.Tests.StateHandlersTests.Helpers;
using FluentAssertions;
using FluentAssertions.Extensions;
using NSubstitute;
using static Core.Tests.StateHandlersTests.Helpers.CircuitBreakerBuilder;

namespace Core.Tests.StateHandlersTests;

public class ClosedCircuitBreakerStateTests
{
    [Fact]
    public void Can_handle_closed_circuit_breaker()
    {
        var saver = Substitute.For<ICircuitBreakerContextUpdater>();
        var systemClock = Substitute.For<ISystemClock>();

        var closedCircuitBreaker = BuildClosedCircuitBreaker();

        var sut = new ClosedCircuitBreakerHandler(saver, systemClock);

        // Act & Assert
        sut.CanHandle(closedCircuitBreaker)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void Cannot_handle_half_open_circuit_breaker()
    {
        var saver = Substitute.For<ICircuitBreakerContextUpdater>();
        var systemClock = Substitute.For<ISystemClock>();

        var halfOpenCircuitBreaker = BuildHalfOpenCircuitBreaker();

        var sut = new ClosedCircuitBreakerHandler(saver, systemClock);

        sut.CanHandle(halfOpenCircuitBreaker)
            .Should().BeFalse();
    }

    [Fact]
    public void Cannot_handle_open_circuit_breaker()
    {
        var saver = Substitute.For<ICircuitBreakerContextUpdater>();
        var systemClock = Substitute.For<ISystemClock>();

        var closedCircuitBreaker = BuildOpenCircuitBreaker();

        var sut = new ClosedCircuitBreakerHandler(saver, systemClock);

        // Act & Assert
        sut.CanHandle(closedCircuitBreaker)
            .Should()
            .BeFalse();
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
        var saverSpy = new CircuitBreakerUpdaterSpy();
        var clock = new SystemClockStub();
        var options = new TestCircuitBreakerSettings();

        options.HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable);

        var circuitBreakerContext = BuildClosedCircuitBreaker()
            .WithAllowedNumberOfFailures(failureAllowedBeforeBreaking)
            .WithFailedCount(currentFailedCount);

        var sut = new ClosedCircuitBreakerHandler(saverSpy, clock);

        // Act
        await sut.Invoking(x => x.HandleAsync<CustomResult>(
                _ => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable),
                circuitBreakerContext,
                options,
                CancellationToken.None))
            .Should()
            .ThrowAsync<CustomHttpException>();

        // Assert
        var savedCircuitBreaker = saverSpy.UpdatedCircuitBreaker;

        savedCircuitBreaker!.ShouldBe()
            .Closed()
            .WithFailedCount(expectedFailedCount);
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
        var saverSpy = new CircuitBreakerUpdaterSpy();
        var clock = new SystemClockStub();
        clock.SetUtcDate(DateTime.UtcNow);

        var options = new TestCircuitBreakerSettings();
        options.HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable);

        var circuitBreakerContext = BuildClosedCircuitBreaker()
            .WithAllowedNumberOfFailures(failureAllowedBeforeBreaking)
            .WithFailedCount(currentFailedCount)
            .UsingSystemClock(clock);

        var sut = new ClosedCircuitBreakerHandler(saverSpy, clock);

        // Act & Assert
        await sut.Invoking(x => x.HandleAsync<CustomResult>(
                _ => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable),
                circuitBreakerContext,
                options,
                CancellationToken.None))
            .Should()
            .ThrowAsync<CustomHttpException>();

        var savedCircuitBreaker = saverSpy.UpdatedCircuitBreaker;

        var transitToHalfOpenStateDate = clock.CurrentUtcTime() + options.DurationOfBreak;

        savedCircuitBreaker!.ShouldBe()
            .NotClosed()
            .WithFailedCount(expectedFailedCount)
            .LastTimeStateChangedAt(clock.CurrentUtcTime())
            .WillTransitToHalfOpenState(transitToHalfOpenStateDate);
    }

    [Fact]
    public async Task CircuitBreaker_is_not_changed_if_no_exception_handling_specified()
    {
        // Arrange
        var saverSpy = new CircuitBreakerUpdaterSpy();
        var clock = new SystemClockStub();
        var options = new TestCircuitBreakerSettings();

        var circuitBreakerContext = BuildClosedCircuitBreaker()
            .WithAllowedNumberOfFailures(3)
            .WithFailedCount(1);

        var sut = new ClosedCircuitBreakerHandler(saverSpy, clock);

        // Act
        await sut.Invoking(x => x.HandleAsync<CustomResult>(
                _ => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable),
                circuitBreakerContext,
                options,
                CancellationToken.None))
            .Should()
            .ThrowAsync<CustomHttpException>();

        // Assert
        var savedCircuitBreaker = saverSpy.UpdatedCircuitBreaker;
        savedCircuitBreaker.Should().BeNull();
    }


    [Fact]
    public void Cannot_handle_circuit_breaker_which_is_not_closed()
    {
        // Arrange
        var saverSpy = new CircuitBreakerUpdaterSpy();
        var clock = new SystemClockStub();
        var options = new TestCircuitBreakerSettings();

        CircuitBreakerContext circuitBreakerContext = BuildOpenCircuitBreaker()
            .WithAllowedNumberOfFailures(3)
            .WithFailedCount(3)
            .LastTimeStateChangedAt(clock.CurrentUtcTime() - 10.Seconds())
            .WillTransitToHalfOpenStateAt(clock.CurrentUtcTime() + 5.Minutes());

        var sut = new ClosedCircuitBreakerHandler(saverSpy, clock);

        // Act
        sut.Invoking(x => x.HandleAsync(
                _ => Task.FromResult(new CustomResult(true)),
                circuitBreakerContext,
                options,
                CancellationToken.None))
            .Should()
            .ThrowAsync<InvalidCircuitBreakerStateException>()
            .Result
            .Which.Should().Match<InvalidCircuitBreakerStateException>
            (ex => ex.ActualState == CircuitBreakerSnapshot.Open &&
                   ex.ExpectedState == CircuitBreakerSnapshot.Closed &&
                   ex.CircuitBreakerName == circuitBreakerContext.Name);

        // Assert
        var savedCircuitBreaker = saverSpy.UpdatedCircuitBreaker;

        savedCircuitBreaker.Should().BeNull();
    }
}