using DCB.Core.Exceptions;
using DCB.Helpers;

namespace DCB.Core.CircuitBreakers.CircuitBreakerContext;

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

    public void Close(DateTime currentTime)
    {
        if (State != CircuitBreakerStateEnum.HalfOpen)
            throw new InvalidCircuitBreakerStateException(Name, State, CircuitBreakerStateEnum.HalfOpen);
        
        State = CircuitBreakerStateEnum.Closed;
        FailedCount = 0;
        TransitionDateToHalfOpenState = null;
        LastTimeStateChanged = currentTime;
    }
    
    public static CircuitBreakerContext BuildFromSnapshot(CircuitBreakerContextSnapshot snapshot, DateTime currentTime)
    {
        snapshot.ThrowIfNull();
        currentTime.ThrowIfDefault();
        snapshot.DurationOfBreak.ThrowIfDefault();
            
        if (snapshot.IsCircuitBreakerClosed)
            return CreateInClosedState(snapshot);

        if (snapshot.TransitionDateToHalfOpenState <= currentTime)
            return CreateInHalfOpenState(snapshot);
        
        return CreateInOpenState(snapshot);
    }

    public CircuitBreakerContextSnapshot GetSnapshot() =>
        new
        (
            Name, 
            FailureAllowedBeforeBreaking, 
            FailedCount, 
            State == CircuitBreakerStateEnum.Closed,
            TransitionDateToHalfOpenState, 
            LastTimeStateChanged, 
            DurationOfBreak
        );
}