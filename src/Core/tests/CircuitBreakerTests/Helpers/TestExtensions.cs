using Core.Policy;
using Extensions.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Tests.CircuitBreakerTests.Helpers;

public static class TestExtensions
{
    public static async Task ShouldBeInStateAsync<TPolicy>(
        this ICircuitBreaker<TPolicy> circuitBreaker,
        CircuitBreakerState expectedState)
        where TPolicy : CircuitBreakerPolicy
    {
        CircuitBreakerState state = await circuitBreaker.GetStateAsync(CancellationToken.None);
        state.Should().Be(expectedState);
    }

    public static ICircuitBreaker<TPolicy> ConfigureAndGetCircuitBreaker<TPolicy>(this ServiceCollection services) 
        where TPolicy : CircuitBreakerPolicy, new()
    {
        services.AddDistributedCircuitBreaker(ops =>
        {
            ops.UseInMemoryStorage()
                .AddCircuitBreaker<TPolicy>();
        });

        return services
            .BuildServiceProvider()
            .GetRequiredService<ICircuitBreaker<TPolicy>>();
    }
    
    public static ICircuitBreaker<TPolicy> ConfigureAndGetCircuitBreaker<TPolicy>(
        this ServiceCollection services, 
        TPolicy policy) 
        where TPolicy : CircuitBreakerPolicy
    {
        services.AddDistributedCircuitBreaker(ops =>
        {
            ops.UseInMemoryStorage()
                .AddCircuitBreaker(policy);
        });

        return services
            .BuildServiceProvider()
            .GetRequiredService<ICircuitBreaker<TPolicy>>();
    }
}