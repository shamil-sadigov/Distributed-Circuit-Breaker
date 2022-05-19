﻿using System.Net;
using DCB.Core.CircuitBreakers;
using DCB.Core.Exceptions;
using DCB.Core.Tests.ResultHandler.Helpers;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DCB.Core.Tests.FunctionalTests;

// TODO: Test uniqueness of circuti Breakers
public class CircuitBreakerTests
{
    [Theory]
    [InlineData(10)]
    [InlineData(20)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task CircuitBreaker_remain_closed_when_action_is_successful(int executionCount)
    {
        // Arrange
        var actionInvokedTimes = 0;
        
        var circuitBreaker = new ServiceCollection()
            .ConfigureAndGetCircuitBreaker<CurrencyExchangeServiceOptions>();
        
        // Act & Assert
        for (var i = 0; i < executionCount; i++)
        {
            await circuitBreaker.ExecuteAsync(_ =>
            {
                actionInvokedTimes++;
                return CurrencyExchangeResponse.SuccessfulResult;
            }, CancellationToken.None);

            await circuitBreaker.ShouldBeInStateAsync(CircuitBreakerState.Closed);
        }

        actionInvokedTimes.Should().Be(executionCount);
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
            .ConfigureAndGetCircuitBreaker<CurrencyExchangeServiceOptions>();
        
        // Act & Assert
        for (var i = 0; i < executionCount; i++)
        {
            await circuitBreaker.Invoking(x => x.ExecuteAsync<CurrencyExchangeResponse>(token =>
            {
                // a ton of logic...
                throw new ArgumentException("Pretty descriptive message");
            }, CancellationToken.None))
            .Should()
            .ThrowAsync<ArgumentException>();

            await circuitBreaker.ShouldBeInStateAsync(CircuitBreakerState.Closed);
        }
    }

    // Write for result and for Exception
    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task CircuitBreaker_is_open_when_thrown_exception_in_action_is_handleable(
        int failureAllowedBeforeBreaking)
    {
        // Arrange
        var options = new CurrencyExchangeServiceOptions(failureAllowedBeforeBreaking, durationOfBreak: 3.Seconds());

        var circuitBreaker = new ServiceCollection()
            .ConfigureAndGetCircuitBreaker(options);

        var actionInvokedTimesInOpenState = 0;
        
        // Act and Assert
        for (var i = 0; i < failureAllowedBeforeBreaking; i++)
            await circuitBreaker.Invoking(
                    x => x.ExecuteAsync<CurrencyExchangeResponse>(
                        _ =>
                        {
                            throw new CustomHttpException(HttpStatusCode.ServiceUnavailable);
                        }, CancellationToken.None))
                .Should()
                .ThrowAsync<CustomHttpException>();

        await circuitBreaker.ShouldBeInStateAsync(CircuitBreakerState.Open);

        await circuitBreaker.Invoking(x => x.ExecuteAsync(_ =>
            {
                actionInvokedTimesInOpenState++;
                return CurrencyExchangeResponse.SuccessfulResult;
            }, CancellationToken.None))
            .Should()
            .ThrowAsync<CircuitBreakerIsOpenException>();

        actionInvokedTimesInOpenState.Should().Be(0, "Because action is not invoked when circuit breaker is open");
    }
    
    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task CircuitBreaker_is_open_when_action_result_is_handleable(int failureAllowedBeforeBreaking)
    {
        // Arrange
        var options = new CurrencyExchangeServiceOptions(failureAllowedBeforeBreaking, durationOfBreak: 10.Seconds());
        var actionInvokedTimesInOpenState = 0;

        var circuitBreaker = new ServiceCollection()
            .ConfigureAndGetCircuitBreaker(options);
        
        // Act and Assert
        for (var i = 0; i < failureAllowedBeforeBreaking; i++)
            await circuitBreaker.ExecuteAsync(_ => CurrencyExchangeResponse.UnsuccessfulResult, CancellationToken.None);

        await circuitBreaker.ShouldBeInStateAsync(CircuitBreakerState.Open);
        
        await circuitBreaker.Invoking(x => x.ExecuteAsync(_ =>
            {
                actionInvokedTimesInOpenState++;
                return CurrencyExchangeResponse.SuccessfulResult;
            }, CancellationToken.None))
            .Should()
            .ThrowAsync<CircuitBreakerIsOpenException>();

        actionInvokedTimesInOpenState.Should().Be(0, "Because action is not invoked when circuit breaker is open");
    }
    
    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task CircuitBreaker_is_switched_from_open_to_half_open_state_after_specified_duration
        (int duration)
    {
        // Arrange
        const int failureAllowedBeforeBreaking = 3;
        
        var options = new CurrencyExchangeServiceOptions(
            failureAllowedBeforeBreaking, 
            durationOfBreak: duration.Seconds());
        
        var actionInvokedTimesInOpenState = 0;
    
        var circuitBreaker = new ServiceCollection()
            .ConfigureAndGetCircuitBreaker(options);
        
        await BringTillOpenState(circuitBreaker, failureAllowedBeforeBreaking);
        
        // Act
        // Let's wait till CircuitBreaker become HalfOpen
        await Task.Delay(duration.Seconds());
    
        // Assert
        await circuitBreaker.ShouldBeInStateAsync(CircuitBreakerState.HalfOpen);
    }

    private static async Task BringTillOpenState(
        ICircuitBreaker<CurrencyExchangeServiceOptions> circuitBreaker, 
        int failureAllowedBeforeBreaking)
    {
        for (var i = 0; i < failureAllowedBeforeBreaking; i++)
            await circuitBreaker.ExecuteAsync(_ => CurrencyExchangeResponse.UnsuccessfulResult, CancellationToken.None);

        await circuitBreaker.ShouldBeInStateAsync(CircuitBreakerState.Open);
    }
}