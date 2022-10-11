using Core.Settings;
using FluentAssertions.Extensions;

namespace Core.Tests.StateHandlersTests.Helpers;

public class TestCircuitBreakerSettings : CircuitBreakerSettings
{
    public override string Name => "Treezor";
    public override int FailureAllowedBeforeBreaking => 2;
    public override TimeSpan DurationOfBreak => 5.Seconds();
}