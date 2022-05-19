using System.Net;
using DCB.Core.CircuitBreakers.StateHandlers;
using DCB.Core.Storage;
using DCB.Core.Tests.ResultHandler.Helpers;
using DCB.Core.Tests.StateHandlers.Helpers;
using FluentAssertions;
using NSubstitute;
using static DCB.Core.Tests.StateHandlers.Helpers.CircuitBreakerBuilder;

namespace DCB.Core.Tests.StateHandlers;

public class HalfOpenCircuitBreakerHandlerTests
{
    [Fact]
    public void Can_handle_half_open_circuit_breaker()
    {
        var saver = Substitute.For<ICircuitBreakerContextUpdater>();
        var systemClock = Substitute.For<ISystemClock>();

        var halfOpenCircuitBreaker = BuildHalfOpenCircuitBreaker();

        var sut = new HalfOpenCircuitBreakerStateHandler(systemClock, saver);

        sut.CanHandle(halfOpenCircuitBreaker)
            .Should().BeTrue();
    }

    [Fact]
    public void Cannot_handle_closed_circuit_breaker()
    {
        var saver = Substitute.For<ICircuitBreakerContextUpdater>();
        var systemClock = Substitute.For<ISystemClock>();

        var closedCircuitBreaker = BuildClosedCircuitBreaker();

        var sut = new HalfOpenCircuitBreakerStateHandler(systemClock, saver);

        // Act & Assert
        sut.CanHandle(closedCircuitBreaker)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Cannot_handle_open_circuit_breaker()
    {
        var saver = Substitute.For<ICircuitBreakerContextUpdater>();
        var systemClock = Substitute.For<ISystemClock>();

        var closedCircuitBreaker = BuildOpenCircuitBreaker();

        var sut = new HalfOpenCircuitBreakerStateHandler(systemClock, saver);

        // Act & Assert
        sut.CanHandle(closedCircuitBreaker)
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    public async Task CircuitBreaker_is_broken_in_case_of_failure(int failedCount)
    {
        // Arrange
        var saverSpy = new CircuitBreakerUpdaterSpy();
        var clock = new SystemClockStub();
        var options = new TestCircuitBreakerOptions();

        clock.SetUtcDate(DateTime.UtcNow);

        options.HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable);

        var circuitBreakerContext = BuildHalfOpenCircuitBreaker()
            .WithFailedCount(failedCount)
            .WithAllowedNumberOfFailures(failedCount)
            .UsingSystemClock(clock);

        var sut = new HalfOpenCircuitBreakerStateHandler(clock, saverSpy);

        // Act
        await sut.Invoking(x => x.HandleAsync<CustomResult>(
                _ => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable),
                circuitBreakerContext, options, CancellationToken.None))
            .Should()
            .ThrowAsync<CustomHttpException>();

        // Assert
        var savedCircuitBreaker = saverSpy.UpdatedCircuitBreaker;

        savedCircuitBreaker!.ShouldBe()
            .NotClosed()
            .WithFailedCount(++failedCount)
            .LastTimeStateChangedAt(clock.GetCurrentTime());
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    public async Task CircuitBreaker_remain_half_open_when_exception_is_not_handled(
        int failedCount)
    {
        // Arrange
        var saverSpy = new CircuitBreakerUpdaterSpy();
        var clock = new SystemClockStub();
        var options = new TestCircuitBreakerOptions();

        clock.SetUtcDate(DateTime.UtcNow);

        var circuitBreakerContext = BuildHalfOpenCircuitBreaker()
            .WithFailedCount(failedCount)
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
        var options = new TestCircuitBreakerOptions();

        options.HandleResult<CustomResult>(x => !x.IsSuccessful);

        clock.SetUtcDate(DateTime.UtcNow);

        var circuitBreakerContext = BuildHalfOpenCircuitBreaker()
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
            .LastTimeStateChangedAt(clock.GetCurrentTime())
            .WillNotTransitToHalfOpenState()
            .WithFailedCount(0);
    }
}