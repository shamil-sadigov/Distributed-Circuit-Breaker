namespace Core.Tests.StateHandlersTests.Helpers;

public class SystemClockStub : ISystemClock
{
    private DateTime? _utcNow;
    
    public void SetUtcDate(DateTime utcNowTime)
    {
        _utcNow = utcNowTime;
    }

    public DateTime CurrentUtcTime => _utcNow ?? DateTime.UtcNow;
}