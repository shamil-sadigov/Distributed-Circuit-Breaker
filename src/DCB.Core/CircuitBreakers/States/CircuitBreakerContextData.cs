namespace DCB.Core.CircuitBreakers.States;

/// <summary>
/// Data of <see cref="CircuitBreakerContext"/>
/// that can be used for persistence purposes and creating <see cref="CircuitBreakerContext"/>
/// </summary>
public class CircuitBreakerContextData
{
    public string Name { get; init; }
    public int FailureAllowedBeforeBreaking { get; init; }
    public int FailedCount { get; init; }
    public bool IsCircuitBreakerClosed { get; set; }
    public DateTime? TransitionDateToHalfOpenState { get; init; }
    public DateTime? LastTimeStateChanged { get; init; }
}