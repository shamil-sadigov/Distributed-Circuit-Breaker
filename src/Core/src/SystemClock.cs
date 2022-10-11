namespace Core;

public class SystemClock:ISystemClock
{
    public DateTime UtcTime => DateTime.UtcNow;
}