using DCB.Core.CircuitBreakers;

namespace DCB.Core.Exceptions;

public class InvalidCircuitBreakerStateException : CircuitBreakerException
{
    public InvalidCircuitBreakerStateException(
        string circuitBreakerName,
        CircuitBreakerState actualState,
        CircuitBreakerState expectedState)
        : base(BuildExceptionMessage(circuitBreakerName, actualState, expectedState))
    {
        CircuitBreakerName = circuitBreakerName;
        ExpectedState = expectedState;
        ActualState = actualState;
    }

    public string CircuitBreakerName { get; }
    public CircuitBreakerState ExpectedState { get; }
    public CircuitBreakerState ActualState { get; }

    private static string BuildExceptionMessage(
        string circuitBreakerName,
        CircuitBreakerState actualState,
        CircuitBreakerState expectedState)
    {
        return $"Expected CircuitBreaker with name '{circuitBreakerName}' " +
               $"to be in {expectedState} state " +
               $"but found in '{actualState}' state";
    }
}