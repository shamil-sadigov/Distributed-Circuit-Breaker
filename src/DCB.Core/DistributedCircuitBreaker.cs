using Polly;

namespace DCB.Core;


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