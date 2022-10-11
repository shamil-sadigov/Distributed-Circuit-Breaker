using Core.CircuitBreakers.Context;

namespace Core.CircuitBreakers;

/// <summary>
///     Data of <see cref="CircuitBreakerContext" />
///     that can be used for persistence purposes and creating <see cref="CircuitBreakerContext" />
/// </summary>
public sealed record CircuitBreakerSnapshot
(
    string Name,
    int FailedCount,
    DateTime? LastTimeFailed
);