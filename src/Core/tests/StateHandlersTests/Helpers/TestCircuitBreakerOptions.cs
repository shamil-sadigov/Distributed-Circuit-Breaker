using Core.CircuitBreakerOption;
using FluentAssertions.Extensions;

namespace Core.Tests.StateHandlersTests.Helpers;

public class TestCircuitBreakerOptions : CircuitBreakerOptions
{
    public override string Name => "Treezor";
    public override int FailureAllowedBeforeBreaking => 2;
    public override TimeSpan DurationOfBreak => 5.Seconds();
}