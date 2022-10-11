namespace Core.Exceptions;

public class InvalidCircuitBreakerStateException : CircuitBreakerException
{
    public InvalidCircuitBreakerStateException(
        string circuitBreakerName,
        string message)
        : base(message)
    {
        CircuitBreakerName = circuitBreakerName;
    }

    public string CircuitBreakerName { get; }
}