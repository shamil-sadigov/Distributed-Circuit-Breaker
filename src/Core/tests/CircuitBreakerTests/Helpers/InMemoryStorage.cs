using System.Collections.Concurrent;
using Core.Storage;

namespace Core.Tests.CircuitBreakerTests.Helpers;

public sealed class InMemoryStorage:ICircuitBreakerStorage
{
    private readonly ConcurrentDictionary<string, CircuitBreakerSnapshot> _snapshotStorage = new();

    public  Task<CircuitBreakerSnapshot?> GetAsync(string circuitBreakerName, CancellationToken token)
    {
        return Task.FromResult(_snapshotStorage.GetValueOrDefault(circuitBreakerName));
    }

    public Task SaveAsync(CircuitBreakerSnapshot snapshot, CancellationToken token)
    {
        _snapshotStorage[snapshot.Name] = snapshot;
        
        return Task.CompletedTask;
    }
}