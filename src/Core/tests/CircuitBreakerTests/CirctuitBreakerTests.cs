using System.Net;
using Core.Exceptions;
using Core.Tests.CircuitBreakerTests.Helpers;
using Core.Tests.ResultHandlerTests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Tests.CircuitBreakerTests;

using static CircuitBreakerState;

// TODO: Test uniqueness of circuti Breakers

// TODO: Use 'handleable' word instead of 'specified'

public class CircuitBreakerTests
{
    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task CircuitBreaker_remain_closed_when_execution_is_successful(int executionCount)
    {
        // Arrange
        var executedTimes = 0;
        
        ICircuitBreaker<ExternalServicePolicy> circuitBreaker = new ServiceCollection()
            .ConfigureAndGetCircuitBreaker(ExternalServicePolicy.WithDefaultTestValues);
        
        // Act & Assert
        for (var i = 0; i < executionCount; i++)
        {
            await circuitBreaker.ExecuteAsync(_ =>
            {
                executedTimes++;
                return ExternalServiceResponse.SuccessfulResult;
            }, CancellationToken.None);

            await circuitBreaker.ShouldBeInStateAsync(Closed);
        }

        executedTimes.Should().Be(executionCount);
    }
    
    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task CircuitBreaker_remain_closed_when_thrown_exception_is_not_handleable(int executionCount)
    {
        // Arrange
        var circuitBreaker =  new ServiceCollection()
            .ConfigureAndGetCircuitBreaker(ExternalServicePolicy.WithDefaultTestValues);
        
        // Act & Assert
        for (var i = 0; i < executionCount; i++)
        {
            await circuitBreaker.Invoking(x => x.ExecuteAsync(_ =>
            {
                // a ton of logic...
                throw new ArgumentException("Pretty descriptive message");
            }, CancellationToken.None))
            .Should()
            .ThrowAsync<ArgumentException>();

            await circuitBreaker.ShouldBeInStateAsync(Closed);
        }
    }

    // Write for result and for Exception
    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task CircuitBreaker_is_open_when_thrown_exception__is_handleable(int failureAllowedBeforeBreaking)
    {
        // Arrange
        var policy = new ExternalServicePolicy(failureAllowedBeforeBreaking, durationOfBreak: 5.Seconds());

        ICircuitBreaker<ExternalServicePolicy> circuitBreaker = new ServiceCollection()
            .ConfigureAndGetCircuitBreaker(policy);
        
        // Act and Assert
        for (var i = 0; i < failureAllowedBeforeBreaking; i++)
            await circuitBreaker.Invoking(
                    x => x.ExecuteAsync(
                        _ => throw new CustomHttpException(HttpStatusCode.ServiceUnavailable), CancellationToken.None))
                .Should()
                .ThrowAsync<CustomHttpException>();

        await circuitBreaker.ShouldBeInStateAsync(Open);

        var executedTimesInOpenState = 0;
        
        await circuitBreaker.Invoking(x => x.ExecuteAsync(_ =>
            {
                executedTimesInOpenState++;
                return ExternalServiceResponse.SuccessfulResult;
            }, CancellationToken.None))
            .Should()
            .ThrowAsync<CircuitBreakerIsOpenException>();

        executedTimesInOpenState.Should().Be(0, "Because execution is not invoked when circuit breaker is open");
    }
    
    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task CircuitBreaker_is_open_when_action_result_is_handleable(int failureAllowedBeforeBreaking)
    {
        var policy = new ExternalServicePolicy(failureAllowedBeforeBreaking, durationOfBreak: 10.Seconds());

        ICircuitBreaker<ExternalServicePolicy> circuitBreaker = new ServiceCollection()
            .ConfigureAndGetCircuitBreaker(policy);
        
        // Act and Assert
        for (var i = 0; i < failureAllowedBeforeBreaking; i++)
            await circuitBreaker.ExecuteAsync(_ => ExternalServiceResponse.UnsuccessfulResult, CancellationToken.None);

        await circuitBreaker.ShouldBeInStateAsync(Open);
        
        var executedTimesInOpenState = 0;

        await circuitBreaker.Invoking(x => x.ExecuteAsync(_ =>
            {
                executedTimesInOpenState++;
                return ExternalServiceResponse.SuccessfulResult;
            }, CancellationToken.None))
            .Should()
            .ThrowAsync<CircuitBreakerIsOpenException>();

        executedTimesInOpenState.Should().Be(0, "Because execution is not invoked when circuit breaker is open");
    }
    
    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task CircuitBreaker_is_switched_from_open_to_halfOpen_state_after_configured_duration(int durationOfBreak)
    {
        // Arrange
        const int failureAllowedBeforeBreaking = 3;

        var policy = new ExternalServicePolicy(failureAllowedBeforeBreaking, durationOfBreak.Seconds());

        ICircuitBreaker<ExternalServicePolicy> circuitBreaker = new ServiceCollection()
            .ConfigureAndGetCircuitBreaker(policy);
        
        await BringTillOpenState(circuitBreaker, failureAllowedBeforeBreaking);
        
        // Act
        // Let's wait till CircuitBreaker become HalfOpen
        await Task.Delay(durationOfBreak.Seconds());
        
        // Assert
        await circuitBreaker.ShouldBeInStateAsync(HalfOpen);
    }
    
    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task When_circuitBreaker_is_halfOpened_and_executed_action_is_successful_then_circuitBreaker_is_closed
        (int durationOfBreak)
    {
        // Arrange
        const int failureAllowedBeforeBreaking = 3;
        var actionInvokedTimes = 0;

        var policy = new ExternalServicePolicy(failureAllowedBeforeBreaking, durationOfBreak.Seconds());

        ICircuitBreaker<ExternalServicePolicy> circuitBreaker = new ServiceCollection()
            .ConfigureAndGetCircuitBreaker(policy);
        
        await BringTillOpenState(circuitBreaker, failureAllowedBeforeBreaking);

        // Let's wait till CircuitBreaker become HalfOpen
        await Task.Delay(durationOfBreak.Seconds());

        await circuitBreaker.ShouldBeInStateAsync(HalfOpen);
        
        // Act
        await circuitBreaker.ExecuteAsync(_ =>
        {
            actionInvokedTimes++;
            return ExternalServiceResponse.SuccessfulResult;
        }, CancellationToken.None);
        
        // Assert
        await circuitBreaker.ShouldBeInStateAsync(Closed);
        actionInvokedTimes.Should().Be(1);
    }
    
    
    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task When_circuitBreaker_is_halfOpened_and_executed_action_is_not_successful_then_circuitBreaker_is_opened
        (int durationOfBreak)
    {
        // Arrange
        const int failureAllowedBeforeBreaking = 3;
        var actionInvokedTimes = 0;

        var policy = new ExternalServicePolicy(failureAllowedBeforeBreaking, durationOfBreak.Seconds());
        
        var circuitBreaker = new ServiceCollection()
            .ConfigureAndGetCircuitBreaker(policy);
        
        await BringTillOpenState(circuitBreaker, failureAllowedBeforeBreaking);

        // Let's wait till CircuitBreaker become HalfOpen
        await Task.Delay(durationOfBreak.Seconds());

        await circuitBreaker.ShouldBeInStateAsync(HalfOpen);
        
        // Act
        await circuitBreaker.Invoking(x => x.ExecuteAsync(_ =>
            {
                actionInvokedTimes++;
                throw new CustomHttpException(HttpStatusCode.TooManyRequests);
            }, CancellationToken.None))
            .Should()
            .ThrowAsync<CustomHttpException>();

        // Assert
        await circuitBreaker.ShouldBeInStateAsync(Open);
        actionInvokedTimes.Should().Be(1);
    }
    
    private static async Task BringTillOpenState(
        ICircuitBreaker<ExternalServicePolicy> circuitBreaker, 
        int failureAllowedBeforeBreaking)
    {
        for (var i = 0; i < failureAllowedBeforeBreaking; i++)
            await circuitBreaker.ExecuteAsync(_ => ExternalServiceResponse.UnsuccessfulResult, CancellationToken.None);

        await circuitBreaker.ShouldBeInStateAsync(Open);
    }
}