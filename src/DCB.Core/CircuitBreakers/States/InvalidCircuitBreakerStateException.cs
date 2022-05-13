namespace DCB.Core.CircuitBreakers.States;

public class InvalidCircuitBreakerStateException:CircuitBreakerException
{
    public string CircuitBreakerName { get; }
    public CircuitBreakerStateEnum ExpectedState { get; }
    public CircuitBreakerStateEnum ActualState { get; }

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