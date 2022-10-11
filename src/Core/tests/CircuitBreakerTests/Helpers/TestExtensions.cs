using Core.Context;
using Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;


namespace Core.Tests.CircuitBreakerTests.Helpers;

// TODO: Extract mongo connection string OR use TestDb instead of Mongo
// TODO: Add Dispose() that will drop database or collection

public static class TestExtensions
{
    public static async Task ShouldBeInStateAsync<TSettings>(
        this ICircuitBreaker<TSettings> circuitBreaker,
        CircuitBreakerState expectedState)
        where TSettings : CircuitBreakerSettings
    {
        CircuitBreakerState state = await circuitBreaker.GetStateAsync(CancellationToken.None);
        state.Should().Be(expectedState);
    }

    public static ICircuitBreaker<TSettings> ConfigureAndGetCircuitBreaker<TSettings>(this ServiceCollection services) 
        where TSettings : CircuitBreakerSettings, new()
    {
        services.AddDistributedCircuitBreaker(ops =>
        {
            ops.UseMongo(x =>
                {
                    x.ConnectionString = "mongodb://localhost:27017";
                })
                .AddCircuitBreaker<TSettings>();
        });

        return services
            .BuildServiceProvider()
            .GetRequiredService<ICircuitBreaker<TSettings>>();
    }
    
    public static ICircuitBreaker<TOptions> ConfigureAndGetCircuitBreaker<TOptions>(
        this ServiceCollection services, 
        TOptions options) 
        where TOptions : CircuitBreakerSettings, new()
    {
        services.AddDistributedCircuitBreaker(ops =>
        {
            ops.UseMongo(x =>
                {
                    x.ConnectionString = "mongodb://localhost:27017";
                })
                .AddCircuitBreaker(options);
        });

        return services
            .BuildServiceProvider()
            .GetRequiredService<ICircuitBreaker<TOptions>>();
    }
}