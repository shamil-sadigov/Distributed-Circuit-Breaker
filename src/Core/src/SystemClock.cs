namespace Core;

public class SystemClock:ISystemClock
{
    public DateTime CurrentUtcTime => DateTime.UtcNow;
}