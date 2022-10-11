namespace Core;

public interface ISystemClock
{
    DateTime UtcTime { get; }
}