using DCB.Helpers;

namespace DCB.Core.CircuitBreakers.States;

public partial class CircuitBreakerContext
{
      private static CircuitBreakerContext CreateInOpenState(CircuitBreakerContextSnapshot snapshot)
    {
        snapshot.FailedCount.ThrowIfLessThan(snapshot.FailureAllowedBeforeBreaking);
        snapshot.LastTimeStateChanged.ThrowIfNull();
        snapshot.TransitionDateToHalfOpenState.ThrowIfNull();

        return new CircuitBreakerContext
        {
            Name = snapshot.Name,
            State = CircuitBreakerStateEnum.Open,
            FailedCount = snapshot.FailedCount,
            FailureAllowedBeforeBreaking = snapshot.FailureAllowedBeforeBreaking,
            LastTimeStateChanged = snapshot.LastTimeStateChanged,
            TransitionDateToHalfOpenState = snapshot.TransitionDateToHalfOpenState,
            DurationOfBreak = snapshot.DurationOfBreak
        };
    }
      
    private static CircuitBreakerContext CreateInClosedState(CircuitBreakerContextSnapshot snapshot)
    {
        snapshot.FailedCount.ThrowIfGreaterOrEqualTo(snapshot.FailureAllowedBeforeBreaking);
        
        return new CircuitBreakerContext
        {
            Name = snapshot.Name,
            State = CircuitBreakerStateEnum.Closed,
            FailedCount = snapshot.FailedCount,
            FailureAllowedBeforeBreaking = snapshot.FailureAllowedBeforeBreaking,
            LastTimeStateChanged = snapshot.LastTimeStateChanged,
            DurationOfBreak = snapshot.DurationOfBreak
        };
    }

    private static CircuitBreakerContext CreateInHalfOpenState(CircuitBreakerContextSnapshot snapshot)
    {
        snapshot.FailedCount.ThrowIfLessThan(snapshot.FailureAllowedBeforeBreaking);
        snapshot.LastTimeStateChanged.ThrowIfNull();
        snapshot.TransitionDateToHalfOpenState.ThrowIfNull();

        return new CircuitBreakerContext
        {
            Name = snapshot.Name,
            State = CircuitBreakerStateEnum.HalfOpen,
            FailedCount = snapshot.FailedCount,
            FailureAllowedBeforeBreaking = snapshot.FailureAllowedBeforeBreaking,
            LastTimeStateChanged = snapshot.LastTimeStateChanged,
            TransitionDateToHalfOpenState = snapshot.TransitionDateToHalfOpenState,
            DurationOfBreak = snapshot.DurationOfBreak
        };
    }
}