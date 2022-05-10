namespace DCB.Extensions.Registry;

public class CircuitBreakerOptionsValidationException:Exception
{
    public CircuitBreakerOptionsValidationException(string errorMessage) : base(errorMessage)
    {
    }
}