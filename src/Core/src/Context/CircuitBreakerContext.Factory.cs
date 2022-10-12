using Core.Policy;
using Helpers;

namespace Core.Context;

public partial class CircuitBreakerContext
{
    internal static CircuitBreakerContext CreateNew(CircuitBreakerPolicy policy, ISystemClock systemClock)
    {
        return new CircuitBreakerContext(policy, failedTimes: 0, null, systemClock);
    }
    
    internal static CircuitBreakerContext BuildFromState(
        CircuitBreakerSnapshot snapshot,
        CircuitBreakerPolicy policy,
        ISystemClock systemClock)
    {
        snapshot.ThrowIfNull();
        return new CircuitBreakerContext(policy, snapshot.FailedTimes, snapshot.LastTimeFailed, systemClock);
    }
}