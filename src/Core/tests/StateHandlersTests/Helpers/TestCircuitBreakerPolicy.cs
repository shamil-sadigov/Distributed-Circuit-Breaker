using Core.Policy;

namespace Core.Tests.StateHandlersTests.Helpers;

public class TestCircuitBreakerPolicy : CircuitBreakerPolicy
{
    public override string Name => "Test";
    public override int FailureAllowed { get; set; }
    public override TimeSpan DurationOfBreak { get; set; }

    public static TestCircuitBreakerPolicy Default = new TestCircuitBreakerPolicy()
    {
        DurationOfBreak = 5.Seconds(),
        FailureAllowed = 3
    };
}