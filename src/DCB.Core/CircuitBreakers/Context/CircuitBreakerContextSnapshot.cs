namespace DCB.Core.CircuitBreakers.Context;

/// <summary>
///     Data of <see cref="CircuitBreakerContext" />
///     that can be used for persistence purposes and creating <see cref="CircuitBreakerContext" />
/// </summary>
public sealed record CircuitBreakerContextSnapshot
(
    string Name,
    int FailureAllowedBeforeBreaking,
    int FailedCount,
    bool IsCircuitBreakerClosed,
    DateTime? TransitionDateToHalfOpenState, // TODO: Rename to transationToLafoOpenStateAt
    DateTime? LastTimeStateChanged,
    TimeSpan DurationOfBreak
);