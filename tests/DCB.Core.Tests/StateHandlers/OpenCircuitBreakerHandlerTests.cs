using System.Net;
using DCB.Core.CircuitBreakers.StateHandlers;
using DCB.Core.CircuitBreakers.States;
using DCB.Core.Tests.ResultHandler.Helpers;
using DCB.Core.Tests.StateHandlers.Helpers;
using FluentAssertions;
using static DCB.Core.Tests.StateHandlers.Helpers.CircuitBreakerBuilder;

namespace DCB.Core.Tests.StateHandlers;

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
         await sut.Invoking(x => x.HandleAsync(
                             options, () => Task.FromResult(new CustomResult(IsSuccessful: true)),
                             circuitBreakerContext))
             .Should()
             .ThrowAsync<CircuitBreakerIsOpenException>();
     }
}