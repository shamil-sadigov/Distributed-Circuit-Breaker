namespace Core.CircuitBreakers.Context;

/// <summary>
///     Data of <see cref="CircuitBreakerContext" />
///     that can be used for persistence purposes and creating <see cref="CircuitBreakerContext" />
/// </summary>
public sealed record CircuitBreakerState
(
    string Name,
    int FailedCount,
    DateTime? LastTimeFailed
);