using Core.Settings;
using Helpers;

namespace Core.Context;

public partial class CircuitBreakerContext
{
    internal static CircuitBreakerContext CreateNew(CircuitBreakerSettings settings, ISystemClock systemClock)
    {
        return new CircuitBreakerContext(settings, failedTimes: 0, null, systemClock);
    }
    
    internal static CircuitBreakerContext BuildFromState(
        CircuitBreakerSnapshot snapshot,
        CircuitBreakerSettings settings,
        ISystemClock systemClock)
    {
        snapshot.ThrowIfNull();
        return new CircuitBreakerContext(settings, snapshot.FailedTimes, snapshot.LastTimeFailed, systemClock);
    }
}