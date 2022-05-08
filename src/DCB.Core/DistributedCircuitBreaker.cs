using Polly;

namespace DCB.Core;

public interface ICircuitBreakerManager
{
    // TODO: Add methods    
}


public class DistributedCircuitBreaker:AsyncPolicy
{
    protected override Task<TResult> ImplementationAsync<TResult>(
        Func<Context, CancellationToken, Task<TResult>> action,
        Context context,
        CancellationToken cancellationToken,
        bool continueOnCapturedContext)
    {
        throw new NotImplementedException();
    }
}