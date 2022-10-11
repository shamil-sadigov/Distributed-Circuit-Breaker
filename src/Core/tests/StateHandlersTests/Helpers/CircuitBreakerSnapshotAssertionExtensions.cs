using Core.Context;

namespace Core.Tests.StateHandlersTests.Helpers;

public static class CircuitBreakerSnapshotAssertionExtensions
{
    public static CircuitBreakerContextAssertion ShouldBe(this CircuitBreakerContext context)
    {
        return new CircuitBreakerContextAssertion(context);
    }
}