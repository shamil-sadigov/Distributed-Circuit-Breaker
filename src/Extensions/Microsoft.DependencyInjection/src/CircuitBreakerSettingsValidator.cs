using Core.Settings;
using Helpers;

namespace Extensions.Microsoft.DependencyInjection;

internal static class CircuitBreakerSettingsValidator
{
    /// <returns>Error messages</returns>
    public static IEnumerable<string> Validate(CircuitBreakerSettings settings)
    {
        if (settings.Name.IsNullOrWhitespace())
            yield return $"{settings.Name} should have a value";

        if (settings.Name.Length > 256)
            yield return $"{settings.Name} should not be longer than 256 characters";
        
        if (settings.DurationOfBreak <= TimeSpan.Zero)
            yield return $"{settings.DurationOfBreak} should be greater than zero";

        if (settings.FailureAllowed < 1)
            yield return $"{settings.FailureAllowed} should be greater than 1";
    }
}