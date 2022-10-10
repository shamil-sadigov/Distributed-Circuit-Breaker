using System.Net;
using Core.CircuitBreakers.StateHandlers;
using Core.Exceptions;
using Core.Tests.ResultHandlerTests.Helpers;
using Core.Tests.StateHandlersTests.Helpers;
using FluentAssertions;
using static Core.Tests.StateHandlersTests.Helpers.CircuitBreakerBuilder;

namespace Core.Tests.StateHandlersTests;

public class OpenCircuitBreakerStateTests
{
    [Fact]
    public void Can_handle_open_circuit_breaker()
    {
        var circuitBreaker = BuildOpenCircuitBreaker();

        var sut = new OpenCircuitBreakerHandler();

        sut.CanHandle(circuitBreaker)
            .Should().BeTrue();
    }

    [Fact]
    public void Cannot_handle_closed_circuit_breaker()
    {
        var closedCircuitBreaker = BuildClosedCircuitBreaker();

        var sut = new OpenCircuitBreakerHandler();

        // Act & Assert
        sut.CanHandle(closedCircuitBreaker)
            .Should().BeFalse();
    }

    [Fact]
    public void Cannot_handle_half_open_circuit_breaker()
    {
        var halfOpenCircuitBreaker = BuildHalfOpenCircuitBreaker();

        var sut = new OpenCircuitBreakerHandler();

        sut.CanHandle(halfOpenCircuitBreaker)
            .Should().BeFalse();
    }

    [Fact]
    public async Task When_circuit_breaker_is_open_then_action_is_impossible_to_invoke()
    {
        // Arrange
        var options = new TestCircuitBreakerOptions();

        options.HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable)
            .HandleResult<CustomResult>(x => !x.IsSuccessful);

        var circuitBreakerContext = BuildOpenCircuitBreaker();

        var sut = new OpenCircuitBreakerHandler();

        // Act & Assert
        await sut.Invoking(x => x.HandleAsync(_ => Task.FromResult(new CustomResult(true)),
                circuitBreakerContext, options, CancellationToken.None))
            .Should()
            .ThrowAsync<CircuitBreakerIsOpenException>();
    }
}