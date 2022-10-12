using System.Net;
using Core.Tests.ResultHandlerTests.Helpers;
using Core.Tests.StateHandlersTests.Helpers;
using static Core.Tests.StateHandlersTests.Helpers.CircuitBreakerBuilder;

namespace Core.Tests.StateHandlersTests;

public class HalfOpenCircuitBreakerHandlerTests
{
    [Fact]
    public void Should_be_able_to_handle_circuit_breaker_only_in_open_state()
    {
        var sut = new HalfOpenCircuitBreakerStateHandler();

        foreach (var circuitBreakerState in Enum.GetValues<CircuitBreakerState>())
            sut.CanHandle(circuitBreakerState)
                .Should()
                .Be(circuitBreakerState is CircuitBreakerState.HalfOpen);
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
        var clock = new SystemClockStub();

        var settings = new TestCircuitBreakerSettings()
        {
            FailureAllowed = failedCount,
            DurationOfBreak = 5.Seconds()
        };

        clock.SetUtcDate(DateTime.UtcNow);

        var circuitBreakerContext = HalfOpenCircuitBreakerWith(settings)
            .WithFailedTimes(failedCount)
            .UsingSystemClock(clock)
            .Build();

        var sut = new HalfOpenCircuitBreakerStateHandler();

        // Act
        await sut.Invoking(x => x.HandleAsync<CustomResult>(
                _ => throw new InvalidOperationException(),
                circuitBreakerContext, CancellationToken.None))
            .Should()
            .ThrowAsync<InvalidOperationException>();


        circuitBreakerContext.ShouldBe()
            .HalfOpen()
            .WillTransitToHalfOpenStateAt(clock.CurrentUtcTime + settings.DurationOfBreak);
    }

    [Fact]
    public async Task CircuitBreaker_is_closed_when_no_failure_happened()
    {
        // Arrange
        var clock = new SystemClockStub();
        
        var settings = new TestCircuitBreakerSettings()
        {
            FailureAllowed = 3,
            DurationOfBreak = 5.Seconds()
        };

        settings.HandleResult<CustomResult>(x => !x.IsSuccessful);

        clock.SetUtcDate(DateTime.UtcNow);

        var circuitBreakerContext = HalfOpenCircuitBreakerWith(settings)
            .UsingSystemClock(clock)
            .Build();

        var sut = new HalfOpenCircuitBreakerStateHandler();

        // Act
        await sut.HandleAsync(
            _ => Task.FromResult(new CustomResult(true)),
            circuitBreakerContext, CancellationToken.None);

        // Assert


        circuitBreakerContext!.ShouldBe()
            .Closed()
            .WillTransitToHalfOpenStateAt(null)
            .WithFailedTimes(0);
    }
}