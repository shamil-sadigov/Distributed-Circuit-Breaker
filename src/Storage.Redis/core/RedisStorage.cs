using System.Text.Json;
using AutoMapper;
using Core;
using Core.Exceptions;
using Core.Storage;
using StackExchange.Redis;

namespace Storage.Redis;

public class RedisStorage : ICircuitBreakerStorage
{
    private readonly RedisDbOptions _options;
    private readonly IMapper _mapper;
    private readonly ConnectionMultiplexer _multiplexer;

    public RedisStorage(RedisDbOptions options, IMapper mapper)
    {
        _options = options;
        _mapper = mapper;
        _multiplexer = ConnectionMultiplexer.Connect(options.ConnectionString!);
    }

    public async Task<CircuitBreakerSnapshot?> GetAsync(string circuitBreakerName, CancellationToken token)
    {
        var dataModel = await GetByNameAsync(circuitBreakerName, token).ConfigureAwait(false);
        return _mapper.Map<CircuitBreakerSnapshot>(dataModel);
    }

    public async Task UpdateAsync(CircuitBreakerSnapshot snapshot, CancellationToken token)
    {
        var dataModel = await GetByNameAsync(snapshot.Name, token).ConfigureAwait(false);

        if (dataModel is null)
            throw new CircuitBreakerSnapshotNotFoundException(snapshot.Name);

        _mapper.Map(snapshot, dataModel);

        var updatedPayload = JsonSerializer.Serialize(dataModel);
        
        await SaveAsync(snapshot.Name, updatedPayload).ConfigureAwait(false);
    }

    public async Task AddAsync(CircuitBreakerSnapshot snapshot, CancellationToken token)
    {
        var dataModel = _mapper.Map<CircuitBreakerDataModel>(snapshot);
        
        var payload = JsonSerializer.Serialize(dataModel);

        await SaveAsync(snapshot.Name, payload).ConfigureAwait(false);
    }

    private async Task SaveAsync(string circuitBreakerName, string payload)
    {
        var db = _multiplexer.GetDatabase();
        
        bool isSet = await db.StringSetAsync(circuitBreakerName, payload).ConfigureAwait(false);
    }

    private async Task<CircuitBreakerDataModel?> GetByNameAsync(string circuitBreakerName, CancellationToken token)
    {
        var db = _multiplexer.GetDatabase();

        string? payload = await db.StringGetAsync(circuitBreakerName);

        return payload is null ? null : JsonSerializer.Deserialize<CircuitBreakerDataModel>(payload);
    }
}