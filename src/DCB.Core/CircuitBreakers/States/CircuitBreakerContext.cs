namespace DCB.Core.CircuitBreakers.States;

public sealed record CircuitBreakerContext
{
    public string Name { get; set; }
    public int FailureAllowedBeforeBreaking { get; init; }
    public int FailedCount { get; set; }
    public CircuitBreakerStateEnum State { get; set; }
    public DateTime? TransitionDateToHalfOpenState { get; set; }
    public DateTime? LastTimeStateChanged { get; set; }
}