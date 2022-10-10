using Core.CircuitBreakers.Context;
using FluentAssertions.Extensions;

namespace Helpers.Tests;

// TODO: Maybe we need to move it
public static class SnapshotHelper
{
    public static CircuitBreakerState CreateSampleSnapshot(string circuitBreakerName)
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

    public static CircuitBreakerState ChangeValues(CircuitBreakerState state)
    {
        return state with
        {
            FailedCount = state.FailedCount + 1,
            FailureAllowedBeforeBreaking = state.FailureAllowedBeforeBreaking + 1,
            IsCircuitBreakerClosed = !state.IsCircuitBreakerClosed,
            TransitionDateToHalfOpenState = state.TransitionDateToHalfOpenState + 1.Minutes(),
            LastTimeFailed = state.LastTimeFailed + 1.Minutes(),
            DurationOfBreak = state.DurationOfBreak + 1.Minutes()
        };
    }
}