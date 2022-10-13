using Core.Policy;
using Shared;

namespace Extensions.Microsoft.DependencyInjection;

internal static class CircuitBreakerPolicyValidator
{
    /// <returns>Error messages</returns>
    public static IEnumerable<string> Validate(CircuitBreakerPolicy policy)
    {
        if (policy.Name.IsNullOrWhitespace())
            yield return $"{policy.Name} should have a value";

        if (policy.Name.Length > 256)
            yield return $"{policy.Name} should not be longer than 256 characters";
        
        if (policy.DurationOfBreak <= TimeSpan.Zero)
            yield return $"{policy.DurationOfBreak} should be greater than zero";

        if (policy.FailureAllowed < 1)
            yield return $"{policy.FailureAllowed} should be greater than 1";
    }
}