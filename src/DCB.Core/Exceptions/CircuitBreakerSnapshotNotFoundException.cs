namespace DCB.Core.Exceptions;

public class CircuitBreakerSnapshotNotFoundException:CircuitBreakerException
{
    /// <summary>
    /// Name of the Circuit Breaker that was not found in storage
    /// </summary>
    public string CircuitBreakerName { get; }

    public CircuitBreakerSnapshotNotFoundException(string circuitBreakerName)
        :base(BuildMessage(circuitBreakerName))
    {
        CircuitBreakerName = circuitBreakerName;
    }

    public static string BuildMessage(string circuitBreakerName) 
        => $"Cannot find circuit breaker with name '{circuitBreakerName}'";
}