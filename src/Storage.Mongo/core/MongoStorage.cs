using AutoMapper;
using Core.Storage;
using MongoDB.Driver;

namespace Storage.Mongo;

public sealed class MongoStorage : ICircuitBreakerStorage
{
    private readonly IMongoCollection<CircuitBreakerDataModel> _circuitBreakerCollection;
    private readonly IMapper _mapper;

    private readonly ReplaceOptions _replaceOptions = new()
    {
        IsUpsert = true
    };
    
    public MongoStorage(
        MongoClient mongoClient,
        MongoDbOptions options,
        IMapper mapper)
    {
        _mapper = mapper;
        _circuitBreakerCollection = mongoClient.GetDatabase(options.DatabaseName)
            .GetCollection<CircuitBreakerDataModel>(options.CollectionName);
    }

    public async Task<CircuitBreakerSnapshot?> GetAsync(string circuitBreakerName, CancellationToken token)
    {
        var dataModel = await GetByNameAsync(circuitBreakerName, token).ConfigureAwait(false);

        return _mapper.Map<CircuitBreakerSnapshot>(dataModel);
    }

    public async Task SaveAsync(CircuitBreakerSnapshot snapshot, CancellationToken token)
    {
        var dataModel = _mapper.Map<CircuitBreakerDataModel>(snapshot);

        await _circuitBreakerCollection
            .ReplaceOneAsync(x => x.Name == snapshot.Name, dataModel, _replaceOptions, cancellationToken: token)
            .ConfigureAwait(false);
    }
    
    private async Task<CircuitBreakerDataModel?> GetByNameAsync(string circuitBreakerName, CancellationToken token)
    {
        var cursor = await _circuitBreakerCollection.FindAsync(
            x => x.Name == circuitBreakerName, cancellationToken: token)
            .ConfigureAwait(false);

        return await cursor.FirstOrDefaultAsync(token).ConfigureAwait(false);
    }
}