namespace Core.Settings;


public interface ICircuitBreakerSettings
{
    public string Name { get; }
    public int FailureAllowedBeforeBreaking { get; }
    public TimeSpan DurationOfBreak { get; }
}