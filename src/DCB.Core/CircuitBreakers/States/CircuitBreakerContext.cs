using DCB.Helpers;

namespace DCB.Core.CircuitBreakers.States;

// TODO: Add tests

public partial class CircuitBreakerContext
{
    private CircuitBreakerContext()
    {
        
    }
    
    public string Name { get; private init; }
    public int FailureAllowedBeforeBreaking { get; private init; }
    public int FailedCount { get; private set; }
    public CircuitBreakerStateEnum State { get; private set; }
    public DateTime? TransitionDateToHalfOpenState { get; private set; }
    public DateTime? LastTimeStateChanged { get; private set; }
    
    /// <summary>
    /// How long CircuitBreaker should remain in open state
    /// </summary>
    public TimeSpan DurationOfBreak { get; private init; }
    
    // TODO: Add unit tests

    public void Failed(DateTime currentTime)
    {
        FailedCount++;

        if (FailedCount < FailureAllowedBeforeBreaking) 
            return;
        
        State = CircuitBreakerStateEnum.Open;
        TransitionDateToHalfOpenState = currentTime + DurationOfBreak;
        LastTimeStateChanged = currentTime;
    }
    
    public static CircuitBreakerContext CreateFromData(CircuitBreakerContextData data, DateTime currentTime)
    {
        data.ThrowIfNull();
        currentTime.ThrowIfNull();
        data.DurationOfBreak.ThrowIfDefault();
            
        if (data.IsCircuitBreakerClosed)
            return CreateInClosedState(data);

        if (data.TransitionDateToHalfOpenState <= currentTime)
            return CreateInHalfOpenState(data);
        
        return CreateInOpenState(data);
    }
}