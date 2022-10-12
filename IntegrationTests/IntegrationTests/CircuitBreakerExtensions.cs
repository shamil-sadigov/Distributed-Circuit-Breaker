using Core;
using Core.Context;
using Core.Settings;

namespace IntegrationTests;

public static class CircuitBreakerExtensions
{
    // Thread blocking is done just for the sake of brevity in unit tests,
    // to not bloat tests with 'await' keyword

    public static bool IsClosed<TSettings>(this ICircuitBreaker<TSettings> circuitBreaker)
        where TSettings : CircuitBreakerSettings
    {
        return circuitBreaker.GetStateAsync(CancellationToken.None).Result == CircuitBreakerState.Closed;
    }
    
    public static bool IsOpen<TSettings>(this ICircuitBreaker<TSettings> circuitBreaker)
        where TSettings : CircuitBreakerSettings
    {
        return circuitBreaker.GetStateAsync(CancellationToken.None).Result == CircuitBreakerState.Open;
    }
    
    public static bool IsHalfOpen<TSettings>(this ICircuitBreaker<TSettings> circuitBreaker)
        where TSettings : CircuitBreakerSettings
    {
        return circuitBreaker.GetStateAsync(CancellationToken.None).Result == CircuitBreakerState.HalfOpen;
    }
}