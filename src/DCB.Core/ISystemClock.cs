namespace DCB.Core;

/// <summary>
/// Register in IoC
/// </summary>
public interface ISystemClock
{
    DateTime CurrentTime { get; }
}