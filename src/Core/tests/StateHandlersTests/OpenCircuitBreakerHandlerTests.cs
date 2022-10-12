using System.Net;
using Core.Exceptions;
using Core.Tests.ResultHandlerTests.Helpers;
using static Core.Tests.StateHandlersTests.Helpers.CircuitBreakerBuilder;

namespace Core.Tests.StateHandlersTests;

public class OpenCircuitBreakerStateTests
{
    [Fact]
    public void Should_be_able_to_handle_circuit_breaker_only_in_open_state()
    {
        var sut = new ClosedCircuitBreakerHandler();

        foreach (var circuitBreakerState in Enum.GetValues<CircuitBreakerState>())
                sut.CanHandle(circuitBreakerState)
                    .Should()
                    .Be(circuitBreakerState is CircuitBreakerState.Open);
    }

    [Fact]
    public async Task When_circuit_breaker_is_open_then_action_is_impossible_to_invoke()
    {
        // Arrange
        var policy = new TestCircuitBreakerPolicy()
        {
            FailureAllowed = 3,
            DurationOfBreak = 5.Seconds()
        };

        policy
            .HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable)
            .HandleResult<CustomResult>(x => !x.IsSuccessful);

        var circuitBreakerContext = OpenCircuitBreakerWith(policy)
            .Build();

        var sut = new OpenCircuitBreakerHandler();

        // Act & Assert
        await sut.Invoking(x => x.HandleAsync(
                _ => Task.FromResult(new CustomResult(true)),
                circuitBreakerContext, 
                CancellationToken.None))
            .Should()
            .ThrowAsync<CircuitBreakerIsOpenException>();
    }
}