using Core.Context;

namespace Core.Logging;

/// <summary>
/// Context that is used for logging only
/// </summary>
internal sealed record CircuitBreakerLoggingContext
(
    string CircuitBreakerName, 
    CircuitBreakerState CircuitBreakerState, 
    int CircuitBreakerFailedTimes,
    int CircuitBreakerFailureAllowed,
    DateTime? TransitionToHalfOpenStateDate
)
{
    public static implicit operator CircuitBreakerLoggingContext(CircuitBreakerContext context) =>
        new
        (
            context.Name,
            context.State,
            context.FailedTimes,
            context.FailureAllowed,
            context.TimeToTransitToHalfOpenState
        );
}