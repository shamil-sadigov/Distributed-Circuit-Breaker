using DCB.Extensions.Registry;

namespace DCB.Extensions;

public class DistributedCircuitBreakerOptions
{
    public CircuitBreakerRegistry CircuitBreakerRegistry { get; set; } = new();
    public OptionsExtensions Extensions { get; set; } = new();
}