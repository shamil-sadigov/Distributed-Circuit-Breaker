using Core.Settings;
using Helpers;

namespace Registration;

public static class CircuitBreakerOptionsValidator
{
    /// <returns>Error messages</returns>
    public static IEnumerable<string> Validate(ICircuitBreakerSettings options)
    {
        if (options.Name.IsNullOrWhitespace())
            yield return $"{options.Name} should have a value";

        if (options.DurationOfBreak <= TimeSpan.Zero)
            yield return $"{options.DurationOfBreak} should be greater than zero";

        if (options.FailureAllowedBeforeBreaking < 1)
            yield return $"{options.FailureAllowedBeforeBreaking} should be greater than 1";
    }
}