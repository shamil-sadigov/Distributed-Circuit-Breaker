using Core.Settings;
using Extensions.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Tests.CircuitBreakerTests.Helpers;

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
            ops.UseInMemoryStorage()
                .AddCircuitBreaker<TSettings>();
        });

        return services
            .BuildServiceProvider()
            .GetRequiredService<ICircuitBreaker<TSettings>>();
    }
    
    public static ICircuitBreaker<TSettings> ConfigureAndGetCircuitBreaker<TSettings>(
        this ServiceCollection services, 
        TSettings options) 
        where TSettings : CircuitBreakerSettings
    {
        services.AddDistributedCircuitBreaker(ops =>
        {
            ops.UseInMemoryStorage()
                .AddCircuitBreaker(options);
        });

        return services
            .BuildServiceProvider()
            .GetRequiredService<ICircuitBreaker<TSettings>>();
    }
}