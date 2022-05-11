namespace DCB.Core.CircuitBreakerOption;

public abstract class CircuitBreakerOptionsBase
{
    public abstract string Name { get; }
    public abstract int ExceptionsAllowedBeforeBreaking { get; }
    public abstract TimeSpan DurationOfBreak { get; }
}