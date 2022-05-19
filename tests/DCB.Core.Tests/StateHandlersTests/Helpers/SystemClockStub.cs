﻿namespace DCB.Core.Tests.StateHandlers.Helpers;

public class SystemClockStub : ISystemClock
{
    private DateTime? _utcNow;
    
    public void SetUtcDate(DateTime utcNowTime)
    {
        _utcNow = utcNowTime;
    }

    public DateTime GetCurrentTime() =>  _utcNow ?? DateTime.UtcNow;
}