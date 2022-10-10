namespace Core.CircuitBreakers;

public class SystemClock:ISystemClock
{
    // TODO: Rename => UtcTime
    public DateTime GetCurrentUtcTime() => DateTime.UtcNow;
}