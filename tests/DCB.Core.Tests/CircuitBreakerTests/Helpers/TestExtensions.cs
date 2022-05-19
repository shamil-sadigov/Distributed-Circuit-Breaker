using DCB.Core.CircuitBreakerOption;
using DCB.Core.CircuitBreakers;
using DCB.Extensions;
using DCB.Extensions.Mongo;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace DCB.Core.Tests.CircuitBreakerTests.Helpers;

// TODO: Extract mongo connection string
// TODO: Add Dispose() that will drop database or collection
public static class TestExtensions
{
    public static async Task ShouldBeInStateAsync<TOptions>(
        this ICircuitBreaker<TOptions> circuitBreaker,
        CircuitBreakerState expectedState)
        where TOptions : CircuitBreakerOptionsBase
    {
        var state = await circuitBreaker.GetStateAsync();
        state.Should().Be(expectedState);
    }

    public static ICircuitBreaker<TOptions> ConfigureAndGetCircuitBreaker<TOptions>(this ServiceCollection services) 
        where TOptions : CircuitBreakerOptions, new()
    {
        services.AddDistributedCircuitBreaker(ops =>
        {
            ops.UseMongo("mongodb://localhost:27017")
                .AddCircuitBreaker<TOptions>();
        });

        return services
            .BuildServiceProvider()
            .GetRequiredService<ICircuitBreaker<TOptions>>();
    }
    
    public static ICircuitBreaker<TOptions> ConfigureAndGetCircuitBreaker<TOptions>(
        this ServiceCollection services, 
        TOptions options) 
        where TOptions : CircuitBreakerOptions, new()
    {
        services.AddDistributedCircuitBreaker(ops =>
        {
            ops.UseMongo("mongodb://localhost:27017")
                .AddCircuitBreaker(options);
        });

        return services
            .BuildServiceProvider()
            .GetRequiredService<ICircuitBreaker<TOptions>>();
    }
}