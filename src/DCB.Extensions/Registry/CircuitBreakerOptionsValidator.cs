using DCB.Core.CircuitBreakerOption;
using DCB.Shared;

namespace DCB.Extensions.Registry;

public class CircuitBreakerOptionsValidator
{
    /// <returns>Error messages</returns>
    public IEnumerable<string> Validate(CircuitBreakerOptionsBase options)
    {
        if (options.Name.IsNullOrWhitespace())
            yield return $"{options.Name} should have a value";

        if (options.DurationOfBreak <= TimeSpan.Zero)
            yield return $"{options.DurationOfBreak} should be greater than zero";

        if (options.FailureAllowedBeforeBreaking < 1)
            yield return $"{options.FailureAllowedBeforeBreaking} should be greater than 1";
    }
}