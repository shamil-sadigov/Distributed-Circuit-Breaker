using Core.Context;

namespace Core.Logging;

public sealed record CircuitBreakerLoggingContext
(
    string CircuitBreakerName, 
    CircuitBreakerState CircuitBreakerState, 
    int CircuitBreakerFailedTimes,
    int CircuitBreakerFailureAllowed,
    DateTime? TransitionToHalfOpenStateDate
)
{
    public static implicit operator CircuitBreakerLoggingContext(CircuitBreakerContext context)
    {
        return new CircuitBreakerLoggingContext(context.Name,
            context.State,
            context.FailedTimes,
            context.FailureAllowed,
            context.TimeToTransitToHalfOpenState);
    }
}