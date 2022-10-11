using Core.Settings;
using FluentAssertions.Extensions;

namespace Core.Tests;

public class TestCircuitBreakerSettings : CircuitBreakerSettings
{
    public override string Name => "Test";
    public override int FailureAllowed { get; set; }
    public override TimeSpan DurationOfBreak { get; set; }

    public static TestCircuitBreakerSettings Default = new TestCircuitBreakerSettings()
    {
        DurationOfBreak = 5.Seconds(),
        FailureAllowed = 3
    };
}