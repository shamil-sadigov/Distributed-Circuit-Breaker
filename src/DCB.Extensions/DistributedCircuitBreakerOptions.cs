using DCB.Core;
using DCB.Extensions.Registry;

namespace DCB.Extensions;

public class DistributedCircuitBreakerOptions
{
    public CircuitBreakerRegistry CircuitBreakerRegistry { get; set; } = new();
    // public OptionsExtensions Extensions { get; set; } = new();
    
    // TODO: Maybe it's worth to rename to something more revealing
    public ICircuitBreakerManager Manager { get; set; }
}