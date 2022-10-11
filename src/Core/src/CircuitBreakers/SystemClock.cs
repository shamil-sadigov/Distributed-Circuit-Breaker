namespace Core.CircuitBreakers;

public class SystemClock:ISystemClock
{
    public DateTime UtcTime => DateTime.UtcNow;
}