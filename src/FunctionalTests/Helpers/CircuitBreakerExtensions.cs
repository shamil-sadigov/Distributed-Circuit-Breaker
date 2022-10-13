using Core;
using Core.Context;
using Core.Policy;

namespace FunctionalTests.Helpers;

public static class CircuitBreakerExtensions
{
    // Thread blocking is done just for the sake of brevity in unit tests,
    // to not bloat tests with 'await' keyword

    public static bool IsClosed<TPolicy>(this ICircuitBreaker<TPolicy> circuitBreaker)
        where TPolicy : CircuitBreakerPolicy
    {
        return circuitBreaker.GetStateAsync(CancellationToken.None).Result == CircuitBreakerState.Closed;
    }
    
    public static int GetFailedTimes<TPolicy>(this ICircuitBreaker<TPolicy> circuitBreaker)
        where TPolicy : CircuitBreakerPolicy
    {
        return circuitBreaker.GetFailedTimesAsync(CancellationToken.None).Result;
    }
    
    public static bool IsOpen<TPolicy>(this ICircuitBreaker<TPolicy> circuitBreaker)
        where TPolicy : CircuitBreakerPolicy
    {
        return circuitBreaker.GetStateAsync(CancellationToken.None).Result == CircuitBreakerState.Open;
    }
    
    public static bool IsHalfOpen<TPolicy>(this ICircuitBreaker<TPolicy> circuitBreaker)
        where TPolicy : CircuitBreakerPolicy
    {
        return circuitBreaker.GetStateAsync(CancellationToken.None).Result == CircuitBreakerState.HalfOpen;
    }
}