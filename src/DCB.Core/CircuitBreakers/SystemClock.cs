namespace DCB.Core.CircuitBreakers;

public class SystemClock:ISystemClock
{
    // TODO: Rename => UtcTime
    public DateTime GetCurrentTime() => DateTime.UtcNow;
}