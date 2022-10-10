using Core;

namespace Extensions.Registry;

public class CircuitBreakerOptionsValidationException : CircuitBreakerException
{
    public CircuitBreakerOptionsValidationException(string errorMessage) : base(errorMessage)
    {
    }
}