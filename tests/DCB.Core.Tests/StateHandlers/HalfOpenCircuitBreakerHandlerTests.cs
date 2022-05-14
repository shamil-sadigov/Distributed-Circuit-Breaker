using System.Net;
using DCB.Core.CircuitBreakers.StateHandlers;
using DCB.Core.CircuitBreakers.States;
using DCB.Core.Tests.ResultHandler.Helpers;
using DCB.Core.Tests.StateHandlers.Helpers;
using FluentAssertions;
using NSubstitute;
using static DCB.Core.Tests.StateHandlers.Helpers.CircuitBreakerBuilder;

namespace DCB.Core.Tests.StateHandlers;

public class HalfOpenCircuitBreakerHandlerTests
{
    
     [Theory]
     [InlineData(3)]
     [InlineData(4)]
     public async Task CircuitBreaker_is_broken_in_case_of_failure(int failedCount)
     {
         // Arrange
         var saverSpy = new CircuitBreakerSaverSpy();
         var clock = new SystemClockStub();
         var options = new TestCircuitBreakerOptions();
         
         clock.SetUtcDate(DateTime.UtcNow);

         options.HandleException<CustomHttpException>(x => x.HttpStatus == HttpStatusCode.ServiceUnavailable);

         var circuitBreakerContext = BuildHalfOpenCircuitBreaker()
             .WithFailedCount(failedCount)
             .WithAllowedNumberOfFailures(failedCount)
             .WithSystemClock(clock);
         
         var sut = new HalfOpenCircuitBreakerStateHandler(clock, saverSpy);
         
         // Act
         await sut.Invoking(x => x.HandleAsync<CustomResult>(
                 options,
                 () => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable),
                 circuitBreakerContext))
             .Should()
             .ThrowAsync<CustomHttpException>();
         
         // Assert
         var savedCircuitBreaker = saverSpy.SavedCircuitBreaker;

         savedCircuitBreaker!.ShouldBe()
             .NotClosed()
             .WithFailedCount(++failedCount)
             .LastTimeStateChangedAt(clock.CurrentTime);
     }
     
     [Theory]
     [InlineData(3)]
     [InlineData(4)]
     public async Task CircuitBreaker_remain_half_open_when_exception_is_not_handled(
         int failedCount)
     {
         // Arrange
         var saverSpy = new CircuitBreakerSaverSpy();
         var clock = new SystemClockStub();
         var options = new TestCircuitBreakerOptions();
         
         clock.SetUtcDate(DateTime.UtcNow);
         
         var circuitBreakerContext = BuildHalfOpenCircuitBreaker()
             .WithFailedCount(failedCount)
             .WithAllowedNumberOfFailures(failedCount)
             .WithSystemClock(clock);
         
         var sut = new HalfOpenCircuitBreakerStateHandler(clock, saverSpy);
         
         // Act
         await sut.Invoking(x => x.HandleAsync<CustomResult>(
                 options,
                 () => throw new InvalidOperationException(),
                 circuitBreakerContext))
             .Should()
             .ThrowAsync<InvalidOperationException>();
         
         // Assert
         var savedCircuitBreaker = saverSpy.SavedCircuitBreaker;

         savedCircuitBreaker.Should().BeNull();
     }
     
     [Fact]
     public async Task CircuitBreaker_is_closed_when_no_failure_happened()
     {
         // Arrange
         var saverSpy = new CircuitBreakerSaverSpy();
         var clock = new SystemClockStub();
         var options = new TestCircuitBreakerOptions();

         options.HandleResult<CustomResult>(x => !x.IsSuccessful);
         
         clock.SetUtcDate(DateTime.UtcNow);
         
         var circuitBreakerContext = BuildHalfOpenCircuitBreaker()
             .WithSystemClock(clock);
         
         var sut = new HalfOpenCircuitBreakerStateHandler(clock, saverSpy);
         
         // Act
          await sut.HandleAsync(
              options, 
              () => Task.FromResult(new CustomResult(IsSuccessful: true)), 
              circuitBreakerContext);
         
         // Assert
         var savedCircuitBreaker = saverSpy.SavedCircuitBreaker;

         savedCircuitBreaker.Should().NotBeNull();

         savedCircuitBreaker!.ShouldBe()
             .Closed()
             .LastTimeStateChangedAt(clock.CurrentTime)
             .WillNotTransitToHalfOpenState();
     }
     
     [Fact]
     public void Cannot_handle_closed_circuit_breaker()
     {
         var saver = Substitute.For<ICircuitBreakerContextSaver>();
         var systemClock = Substitute.For<ISystemClock>();
         
         var closedCircuitBreaker = BuildClosedCircuitBreaker();
        
         var sut = new HalfOpenCircuitBreakerStateHandler(systemClock, saver);

         // Act & Assert
         sut.CanHandle(closedCircuitBreaker)
             .Should()
             .BeFalse();
     }
     
     [Fact]
     public void Can_handle_half_open_circuit_breaker()
     {
         var saver = Substitute.For<ICircuitBreakerContextSaver>();
         var systemClock = Substitute.For<ISystemClock>();
         
         var halfOpenCircuitBreaker = BuildHalfOpenCircuitBreaker();
        
         var sut = new HalfOpenCircuitBreakerStateHandler(systemClock, saver);
         
         sut.CanHandle(halfOpenCircuitBreaker)
             .Should().BeTrue();
     }
     
     [Fact]
     public void Cannot_handle_open_circuit_breaker()
     {
         var saver = Substitute.For<ICircuitBreakerContextSaver>();
         var systemClock = Substitute.For<ISystemClock>();
         
         var closedCircuitBreaker = BuildOpenCircuitBreaker();
        
         var sut = new HalfOpenCircuitBreakerStateHandler(systemClock, saver);

         // Act & Assert
         sut.CanHandle(closedCircuitBreaker)
             .Should()
             .BeFalse();
     }
}