using DCB.Core.CircuitBreakers;

namespace DCB.Core.Exceptions;

public class InvalidCircuitBreakerStateException : CircuitBreakerException
{
    public InvalidCircuitBreakerStateException(
        string circuitBreakerName,
        CircuitBreakerStateEnum actualState,
        CircuitBreakerStateEnum expectedState)
        : base(BuildExceptionMessage(circuitBreakerName, actualState, expectedState))
    {
        CircuitBreakerName = circuitBreakerName;
        ExpectedState = expectedState;
        ActualState = actualState;
    }

    public string CircuitBreakerName { get; }
    public CircuitBreakerStateEnum ExpectedState { get; }
    public CircuitBreakerStateEnum ActualState { get; }

    private static string BuildExceptionMessage(
        string circuitBreakerName,
        CircuitBreakerStateEnum actualState,
        CircuitBreakerStateEnum expectedState)
    {
        return $"Expected CircuitBreaker with name '{circuitBreakerName}' " +
               $"to be in {expectedState} state " +
               $"but found in '{actualState}' state";
    }
}