using DCB.Helpers;

namespace DCB.Core.CircuitBreakers.States;

public partial class CircuitBreakerContext
{
      private static CircuitBreakerContext CreateInOpenState(CircuitBreakerContextData data)
    {
        data.FailedCount.ThrowIfLessThan(data.FailureAllowedBeforeBreaking);
        data.LastTimeStateChanged.ThrowIfNull();
        data.TransitionDateToHalfOpenState.ThrowIfNull();

        return new CircuitBreakerContext
        {
            Name = data.Name,
            State = CircuitBreakerStateEnum.Open,
            FailedCount = data.FailedCount,
            FailureAllowedBeforeBreaking = data.FailureAllowedBeforeBreaking,
            LastTimeStateChanged = data.LastTimeStateChanged,
            TransitionDateToHalfOpenState = data.TransitionDateToHalfOpenState,
            DurationOfBreak = data.DurationOfBreak
        };
    }
      
    private static CircuitBreakerContext CreateInClosedState(CircuitBreakerContextData data)
    {
        data.FailedCount.ThrowIfGreaterOrEqualTo(data.FailureAllowedBeforeBreaking);
        
        return new CircuitBreakerContext
        {
            Name = data.Name,
            State = CircuitBreakerStateEnum.Closed,
            FailedCount = data.FailedCount,
            FailureAllowedBeforeBreaking = data.FailureAllowedBeforeBreaking,
            LastTimeStateChanged = data.LastTimeStateChanged,
            TransitionDateToHalfOpenState = data.TransitionDateToHalfOpenState,
            DurationOfBreak = data.DurationOfBreak
        };
    }

    private static CircuitBreakerContext CreateInHalfOpenState(CircuitBreakerContextData data)
    {
        data.FailedCount.ThrowIfLessThan(data.FailureAllowedBeforeBreaking);
        data.LastTimeStateChanged.ThrowIfNull();
        data.TransitionDateToHalfOpenState.ThrowIfNull();

        return new CircuitBreakerContext
        {
            Name = data.Name,
            State = CircuitBreakerStateEnum.HalfOpen,
            FailedCount = data.FailedCount,
            FailureAllowedBeforeBreaking = data.FailureAllowedBeforeBreaking,
            LastTimeStateChanged = data.LastTimeStateChanged,
            TransitionDateToHalfOpenState = data.TransitionDateToHalfOpenState,
            DurationOfBreak = data.DurationOfBreak
        };
    }
}