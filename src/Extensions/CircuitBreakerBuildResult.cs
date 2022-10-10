using Core.CircuitBreakerOption;

namespace Extensions;

public class CircuitBreakerBuildResult
{
    public CircuitBreakerBuildResult(IReadOnlyCollection<CircuitBreakerOptionsBase> circuitBreakerOptions)
    {
        if (circuitBreakerOptions.Count == 0)
            throw new ArgumentException("Should contain at least one option", nameof(circuitBreakerOptions));

        CircuitBreakerOptions = circuitBreakerOptions;
    }

    public IReadOnlyCollection<CircuitBreakerOptionsBase> CircuitBreakerOptions { get; }
    public OptionsExtensions Extensions { get; } = new();
}