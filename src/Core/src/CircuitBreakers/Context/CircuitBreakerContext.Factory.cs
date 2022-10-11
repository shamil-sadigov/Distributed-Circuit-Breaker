using Core.CircuitBreakerOption;
using Helpers;

namespace Core.CircuitBreakers.Context;

public partial class CircuitBreakerContext
{
    internal static CircuitBreakerContext CreateNew(ICircuitBreakerSettings settings, ISystemClock systemClock)
    {
        return new CircuitBreakerContext(settings, failedCount: 0, null, systemClock);
    }
    
    internal static CircuitBreakerContext BuildFromState(
        CircuitBreakerSnapshot snapshot,
        ICircuitBreakerSettings settings,
        ISystemClock systemClock)
    {
        snapshot.ThrowIfNull();
        return new CircuitBreakerContext(settings, snapshot.FailedCount, snapshot.LastTimeFailed, systemClock);
    }
}