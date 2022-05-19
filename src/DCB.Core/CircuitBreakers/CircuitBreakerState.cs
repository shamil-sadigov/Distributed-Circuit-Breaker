namespace DCB.Core.CircuitBreakers;

public enum CircuitBreakerState
{
    None,
    Open,
    HalfOpen,
    Closed
}