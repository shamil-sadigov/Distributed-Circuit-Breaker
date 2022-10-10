namespace Core.CircuitBreakers;

internal struct Void
{
    internal static readonly Task<Void> Instance = Task.FromResult(new Void());
}