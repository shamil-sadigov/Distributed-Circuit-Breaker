using Core.Settings;
using FluentAssertions.Extensions;

namespace Core.Tests;

public class TestSettings : CircuitBreakerSettings
{
    public override string Name => "Test";
    public override int FailureAllowedBeforeBreaking => 3;
    public override TimeSpan DurationOfBreak => 5.Seconds();
}