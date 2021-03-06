using DCB.Core.CircuitBreakerOption;
using FluentAssertions.Extensions;

namespace DCB.Core.Tests;

public class TestOptions : CircuitBreakerOptions
{
    public override string Name => "Test";
    public override int FailureAllowedBeforeBreaking => 3;
    public override TimeSpan DurationOfBreak => 5.Seconds();
}