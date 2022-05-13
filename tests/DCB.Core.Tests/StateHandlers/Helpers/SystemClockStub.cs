using DCB.Core.CircuitBreakers.States;

namespace DCB.Core.Tests.StateHandlers.Helpers;

public class SystemClockStub:ISystemClock
{
    private DateTime? _utcNow;
    
    public DateTime UtcNow => _utcNow ?? DateTime.UtcNow;

    public void SetUtcDate(DateTime utcNowTime)
    {
        _utcNow = utcNowTime;
    }
}