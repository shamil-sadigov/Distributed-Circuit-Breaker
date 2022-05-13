using DCB.Helpers;

namespace DCB.Core.CircuitBreakers.States;

public partial class CircuitBreakerContext
{
    private CircuitBreakerContext()
    {
        
    }
    
    public string Name { get; private init; }
    public int FailureAllowedBeforeBreaking { get; private init; }
    public int FailedCount { get; private init; }
    public CircuitBreakerStateEnum State { get; private init; }
    public DateTime? TransitionDateToHalfOpenState { get; private init; }
    public DateTime? LastTimeStateChanged { get; private init; }
    
    // TODO: Add unit tests
    public static CircuitBreakerContext CreateFromData(CircuitBreakerContextData data, ISystemClock systemClock)
    {
        data.ThrowIfNull();

        if (data.IsCircuitBreakerClosed)
            return CreateInClosedState(data);

        if (data.TransitionDateToHalfOpenState > systemClock.UtcNow)
            return CreateInHalfOpenState(data);
        
        return CreateInOpenState(data);
    }
}