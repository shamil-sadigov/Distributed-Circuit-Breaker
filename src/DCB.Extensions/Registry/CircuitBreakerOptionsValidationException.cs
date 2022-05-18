using DCB.Core;

namespace DCB.Extensions.Registry;

public class CircuitBreakerOptionsValidationException : CircuitBreakerException
{
    public CircuitBreakerOptionsValidationException(string errorMessage) : base(errorMessage)
    {
    }
}