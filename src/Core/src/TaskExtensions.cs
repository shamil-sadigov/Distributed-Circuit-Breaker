using Core.Context;
using Core.Exceptions;
using Core.Storage;

namespace Core;

public static class TaskExtensions
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
        await storage.UpdateAsync(snapshot, token).ConfigureAwait(false);
        return parentTask;
    }
}