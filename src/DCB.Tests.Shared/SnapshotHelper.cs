using DCB.Core.CircuitBreakers.Context;
using FluentAssertions.Extensions;

namespace DCB.Tests.Shared;

public static class SnapshotHelper
{
    public static CircuitBreakerContextSnapshot CreateSampleSnapshot(string circuitBreakerName) =>
        new
        (
            Name: circuitBreakerName,
            FailureAllowedBeforeBreaking: 5,
            FailedCount: 5,
            IsCircuitBreakerClosed: false,
            TransitionDateToHalfOpenState: 19.May(2022).At(22, 00, 00).ToUniversalTime(),
            LastTimeStateChanged: 19.May(2022).At(21, 55, 00).ToUniversalTime(),
            DurationOfBreak: 5.Minutes()
        );
    
    public static CircuitBreakerContextSnapshot ChangeValues(CircuitBreakerContextSnapshot snapshot) =>
        snapshot with
        {
            FailedCount = snapshot.FailedCount + 1,
            FailureAllowedBeforeBreaking = snapshot.FailureAllowedBeforeBreaking + 1,
            IsCircuitBreakerClosed = !snapshot.IsCircuitBreakerClosed,
            TransitionDateToHalfOpenState = snapshot.TransitionDateToHalfOpenState + 1.Minutes(),
            LastTimeStateChanged = snapshot.LastTimeStateChanged + 1.Minutes(),
            DurationOfBreak = snapshot.DurationOfBreak + 1.Minutes(),
        };

}