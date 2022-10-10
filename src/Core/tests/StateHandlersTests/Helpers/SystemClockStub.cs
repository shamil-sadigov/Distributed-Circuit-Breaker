namespace Core.Tests.StateHandlersTests.Helpers;

public class SystemClockStub : ISystemClock
{
    private DateTime? _utcNow;
    
    public void SetUtcDate(DateTime utcNowTime)
    {
        _utcNow = utcNowTime;
    }

    public DateTime GetCurrentUtcTime() =>  _utcNow ?? DateTime.UtcNow;
}