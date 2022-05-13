namespace DCB.Core.CircuitBreakers.States;

/// <summary>
/// Register in IoC
/// </summary>
public interface ISystemClock
{
    DateTime CurrentTime { get; }
}