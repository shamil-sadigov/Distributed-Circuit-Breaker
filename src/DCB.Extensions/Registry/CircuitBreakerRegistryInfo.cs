namespace DCB.Extensions;

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
        Name = name;
        ExceptionsAllowedBeforeBreaking = exceptionsAllowedBeforeBreaking;
        DurationOfBreak = durationOfBreak;
    }
}