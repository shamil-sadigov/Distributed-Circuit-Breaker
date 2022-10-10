namespace Core;

/// <summary>
///     Base Exception for all other exceptions
/// </summary>
public class CircuitBreakerException : Exception
{
    protected CircuitBreakerException(string? message) : base(message)
    {
    }
}