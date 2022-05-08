namespace DCB.Extensions.Registry;

public class CircuitBreakerRegistryInfo
{
    public  string Name { get; }
    public  int ExceptionsAllowedBeforeBreaking { get; }
    public  TimeSpan DurationOfBreak { get; }

    public  CircuitBreakerRegistryInfo(
        string name, 
        int exceptionsAllowedBeforeBreaking,
        TimeSpan durationOfBreak)
    {
        // TODO: Add validation
        
        Name = name;
        ExceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking;
        DurationOfBreak = durationOfBreak;
    }
}