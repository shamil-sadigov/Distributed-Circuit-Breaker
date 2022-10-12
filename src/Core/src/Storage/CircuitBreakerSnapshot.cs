using Core.Context;

namespace Core.Storage;

/// <summary>
///     Representation of <see cref="CircuitBreakerContext" /> that
///     can be used for saving in <see cref="ICircuitBreakerStorage"/>
/// </summary>
public sealed record CircuitBreakerSnapshot
(
    string Name,
    int FailedTimes,
    DateTime? LastTimeFailed
);