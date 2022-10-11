using System.Net;
using Core.Context;
using Core.Exceptions;
using Core.StateHandlers;
using Core.Tests.ResultHandlerTests.Helpers;
using FluentAssertions;
using static Core.Tests.StateHandlersTests.Helpers.CircuitBreakerBuilder;

namespace Core.Tests.StateHandlersTests;

public class OpenCircuitBreakerStateTests
{
    [Fact]
    public void Can_handle_circuit_breaker_only_in_open_state()
    {
        var sut = new ClosedCircuitBreakerHandler();

        foreach (var circuitBreakerState in Enum.GetValues<CircuitBreakerState>())
            if (circuitBreakerState is CircuitBreakerState.HalfOpen)
                sut.CanHandle(circuitBreakerState).Should().BeTrue();
            else
                sut.CanHandle(circuitBreakerState).Should().BeFalse();
    }

    [Fact]
    public async Task When_circuit_breaker_is_open_then_action_is_impossible_to_invoke()
    {
        // Arrange
        var options = new TestCircuitBreakerSettings();

        options.HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable)
            .HandleResult<CustomResult>(x => !x.IsSuccessful);

        var circuitBreakerContext = OpenCircuitBreakerWith();

        var sut = new OpenCircuitBreakerHandler();

        // Act & Assert
        await sut.Invoking(x => x.HandleAsync(_ => Task.FromResult(new CustomResult(true)),
                circuitBreakerContext, options, CancellationToken.None))
            .Should()
            .ThrowAsync<CircuitBreakerIsOpenException>();
    }
}