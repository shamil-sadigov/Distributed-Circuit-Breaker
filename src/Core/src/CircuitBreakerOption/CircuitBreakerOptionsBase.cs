namespace Core.CircuitBreakerOption;

public abstract class CircuitBreakerOptionsBase
{
    public abstract string Name { get; }
    public abstract int FailureAllowedBeforeBreaking { get; }
    public abstract TimeSpan DurationOfBreak { get; }
}