using Core.Context;
using Core.Exceptions;
using Core.Storage;

namespace Core;

public static class Extensions
{
    /// <returns>Returns parent task <see cref="parentTask"/></returns>
    public static async Task<Task<T>> ContinueWithSavingContext<T>(
        this Task<T> parentTask,
        CircuitBreakerContext context,
        ICircuitBreakerStorage storage,
        CancellationToken token)
    {
        if (parentTask.IsFaulted && parentTask.Exception?.InnerException is CircuitBreakerIsOpenException)
        {
            return parentTask;
        }

        var snapshot = context.CreateSnapshot();
        await storage.SaveAsync(snapshot, token).ConfigureAwait(false);
        return parentTask;
    }

    public static void EnsureStateIs(this CircuitBreakerContext circuitBreaker, CircuitBreakerState expectedState)
    {
        if (circuitBreaker.State != expectedState)
        {
            throw new InvalidCircuitBreakerStateException(
                circuitBreaker.Name,
                $"Expected circuit breaker in '{expectedState}' state but got in '{circuitBreaker.State}' state");
        }
    }
}