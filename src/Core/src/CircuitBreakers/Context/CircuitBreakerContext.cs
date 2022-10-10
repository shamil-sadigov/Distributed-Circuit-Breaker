using Core.CircuitBreakerOption;
using Core.Exceptions;
using Helpers;

namespace Core.CircuitBreakers.Context;

// TODO: Add more unit tests

public partial class CircuitBreakerContext
{
    private readonly ISystemClock _systemClock;

    private CircuitBreakerContext(ISystemClock systemClock)
    {
        _systemClock = systemClock;
    }

    public string Name => State.Name;
    public CircuitBreakerState State { get; private set; }
    public CircuitBreakerOptionsBase Options { get; private set; }

    public bool IsClosed()
    {
       return State.FailedCount < Options.FailureAllowedBeforeBreaking; 
    }

    public bool IsOpen()
    {
        var durationOfBreakPassed = (State.LastTimeFailed + Options.DurationOfBreak) > _systemClock.GetCurrentUtcTime();

        return State.FailedCount >= Options.FailureAllowedBeforeBreaking & !durationOfBreakPassed;
    }
    
    public bool IsHalfOpen()
    {
        var durationOfBreakPassed = (State.LastTimeFailed + Options.DurationOfBreak) > _systemClock.GetCurrentUtcTime();

        return State.FailedCount >= Options.FailureAllowedBeforeBreaking & durationOfBreakPassed;
    }
    
    public void Failed()
    {
        State = State with
        {
            FailedCount = State.FailedCount + 1,
            LastTimeFailed = _systemClock.GetCurrentUtcTime()
        };
    }

    public void Reset()
    {
        State = State with
        {
            FailedCount = 0,
            LastTimeFailed = null
        };
    }
}