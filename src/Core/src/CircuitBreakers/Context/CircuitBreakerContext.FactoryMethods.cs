using Helpers;

namespace Core.CircuitBreakers.Context;

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
            State = CircuitBreakerState.Open,
            FailedCount = snapshot.FailedCount,
            FailureAllowedBeforeBreaking = snapshot.FailureAllowedBeforeBreaking,
            LastTimeStateChanged = snapshot.LastTimeStateChanged,
            TransitionDateToHalfOpenState = snapshot.TransitionDateToHalfOpenState,
            DurationOfBreak = snapshot.DurationOfBreak
        };
    }

    internal static CircuitBreakerContext CreateNew(
        string name,
        int failureAllowedBeforeBreaking,
        TimeSpan durationOfBreak)
    {
        name.ThrowIfNull();
        failureAllowedBeforeBreaking.ThrowIfLessOrEqualTo(0);
        durationOfBreak.ThrowIfLessOrEqualTo(TimeSpan.Zero);

        return new CircuitBreakerContext
        {
            Name = name,
            State = CircuitBreakerState.Closed,
            FailureAllowedBeforeBreaking = failureAllowedBeforeBreaking,
            DurationOfBreak = durationOfBreak
        };
    }


    private static CircuitBreakerContext CreateInClosedState(CircuitBreakerContextSnapshot snapshot)
    {
        snapshot.FailedCount.ThrowIfGreaterOrEqualTo(snapshot.FailureAllowedBeforeBreaking);

        return new CircuitBreakerContext
        {
            Name = snapshot.Name,
            State = CircuitBreakerState.Closed,
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
            State = CircuitBreakerState.HalfOpen,
            FailedCount = snapshot.FailedCount,
            FailureAllowedBeforeBreaking = snapshot.FailureAllowedBeforeBreaking,
            LastTimeStateChanged = snapshot.LastTimeStateChanged,
            TransitionDateToHalfOpenState = snapshot.TransitionDateToHalfOpenState,
            DurationOfBreak = snapshot.DurationOfBreak
        };
    }
}