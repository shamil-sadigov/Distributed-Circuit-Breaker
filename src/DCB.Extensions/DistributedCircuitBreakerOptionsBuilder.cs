using DCB.Core;
using DCB.Extensions.Registry;

namespace DCB.Extensions;

public class DistributedCircuitBreakerOptionsBuilder
{
    private readonly DistributedCircuitBreakerOptions _options;
    
    public DistributedCircuitBreakerOptionsBuilder()
    {
        _options = new DistributedCircuitBreakerOptions();
    }

    public DistributedCircuitBreakerOptionsBuilder RegisterCircuitBreaker(
        string circuitBreakerName, 
        int numberOfExceptionsAllowedBeforeBreaking,
        TimeSpan durationOfBreak)
    {
        circuitBreakerName.ThrowIfNull();
        numberOfExceptionsAllowedBeforeBreaking.ThrowIfLessThan(1);
        durationOfBreak.ThrowIfLessThan(TimeSpan.Zero);

        var info = new CircuitBreakerRegistryInfo(
            circuitBreakerName, 
            numberOfExceptionsAllowedBeforeBreaking, 
            durationOfBreak);

        _options.CircuitBreakerRegistry.Add(info);
        
        return this;
    }
}