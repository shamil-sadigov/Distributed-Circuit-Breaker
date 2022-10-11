namespace Core;

public interface ISystemClock
{
    DateTime CurrentUtcTime { get; }
}