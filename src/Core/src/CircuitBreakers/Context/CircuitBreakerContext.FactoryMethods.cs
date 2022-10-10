using Core.CircuitBreakerOption;
using Helpers;

namespace Core.CircuitBreakers.Context;

public partial class CircuitBreakerContext
{
    internal static CircuitBreakerContext CreateNew(CircuitBreakerOptionsBase options, ISystemClock systemClock)
    {
        options.ThrowIfNull();
        
        return new CircuitBreakerContext(systemClock)
        {
            Options = options,
            State = new CircuitBreakerState(options.Name, FailedCount: 0, null)
        };
    }
    
    public static CircuitBreakerContext Build(
        CircuitBreakerOptionsBase options,
        CircuitBreakerState state,
        ISystemClock systemClock)
    {
        options.ThrowIfNull();
        state.ThrowIfNull();

        return new CircuitBreakerContext(systemClock)
        {
            State = state,
            Options = options
        };
    }
}