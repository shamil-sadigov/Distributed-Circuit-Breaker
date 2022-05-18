using DCB.Core.CircuitBreakers.Context;
using FluentAssertions.Extensions;

namespace DCB.Tests.Shared;

public static class SnapshotHelper
{
    public static CircuitBreakerContextSnapshot CreateSampleSnapshot(string circuitBreakerName)
    {
        return new
        (
            circuitBreakerName,
            5,
            5,
            false,
            19.May(2022).At(22, 00).ToUniversalTime(),
            19.May(2022).At(21, 55).ToUniversalTime(),
            5.Minutes()
        );
    }

    public static CircuitBreakerContextSnapshot ChangeValues(CircuitBreakerContextSnapshot snapshot)
    {
        return snapshot with
        {
            FailedCount = snapshot.FailedCount + 1,
            FailureAllowedBeforeBreaking = snapshot.FailureAllowedBeforeBreaking + 1,
            IsCircuitBreakerClosed = !snapshot.IsCircuitBreakerClosed,
            TransitionDateToHalfOpenState = snapshot.TransitionDateToHalfOpenState + 1.Minutes(),
            LastTimeStateChanged = snapshot.LastTimeStateChanged + 1.Minutes(),
            DurationOfBreak = snapshot.DurationOfBreak + 1.Minutes()
        };
    }
}