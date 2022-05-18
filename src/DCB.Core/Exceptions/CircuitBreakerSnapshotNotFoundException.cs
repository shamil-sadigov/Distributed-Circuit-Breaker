namespace DCB.Core.Exceptions;

public class CircuitBreakerSnapshotNotFoundException : CircuitBreakerException
{
    public CircuitBreakerSnapshotNotFoundException(string circuitBreakerName)
        : base(BuildMessage(circuitBreakerName))
    {
        CircuitBreakerName = circuitBreakerName;
    }

    /// <summary>
    ///     Name of the Circuit Breaker that was not found in storage
    /// </summary>
    public string CircuitBreakerName { get; }

    public static string BuildMessage(string circuitBreakerName)
    {
        return $"Cannot find circuit breaker with name '{circuitBreakerName}'";
    }
}