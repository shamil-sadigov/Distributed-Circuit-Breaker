using System.Net;
using DCB.Core.CircuitBreakers;
using DCB.Core.CircuitBreakers.Context;
using DCB.Core.CircuitBreakers.StateHandlers;
using DCB.Core.Exceptions;
using DCB.Core.Storage;
using DCB.Core.Tests.ResultHandler.Helpers;
using DCB.Core.Tests.StateHandlers.Helpers;
using FluentAssertions;
using FluentAssertions.Extensions;
using NSubstitute;
using static DCB.Core.Tests.StateHandlers.Helpers.CircuitBreakerBuilder;

namespace DCB.Core.Tests.StateHandlers;

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
        var options = new TestCircuitBreakerOptions();

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

        var options = new TestCircuitBreakerOptions();
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

        var transitToHalfOpenStateDate = clock.CurrentTime + options.DurationOfBreak;

        savedCircuitBreaker!.ShouldBe()
            .NotClosed()
            .WithFailedCount(expectedFailedCount)
            .LastTimeStateChangedAt(clock.CurrentTime)
            .WillTransitToHalfOpenState(transitToHalfOpenStateDate);
    }

    [Fact]
    public async Task CircuitBreaker_is_not_changed_if_no_exception_handling_specified()
    {
        // Arrange
        var saverSpy = new CircuitBreakerUpdaterSpy();
        var clock = new SystemClockStub();
        var options = new TestCircuitBreakerOptions();

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
        var options = new TestCircuitBreakerOptions();

        CircuitBreakerContext circuitBreakerContext = BuildOpenCircuitBreaker()
            .WithAllowedNumberOfFailures(3)
            .WithFailedCount(3)
            .LastTimeStateChangedAt(clock.CurrentTime - 10.Seconds())
            .WillTransitToHalfOpenStateAt(clock.CurrentTime + 5.Minutes());

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
            (ex => ex.ActualState == CircuitBreakerState.Open &&
                   ex.ExpectedState == CircuitBreakerState.Closed &&
                   ex.CircuitBreakerName == circuitBreakerContext.Name);

        // Assert
        var savedCircuitBreaker = saverSpy.UpdatedCircuitBreaker;

        savedCircuitBreaker.Should().BeNull();
    }
}