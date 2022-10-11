using AutoMapper;
using Core.CircuitBreakers;
using Core.Exceptions;
using Core.Storage;
using MongoDB.Driver;

namespace Registration.Mongo;

public class MongoStorage : ICircuitBreakerStorage
{
    private readonly IMongoCollection<CircuitBreakerDataModel> _circuitBreakerCollection;
    private readonly IMapper _mapper;

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

    public async Task UpdateAsync(CircuitBreakerSnapshot snapshot, CancellationToken token)
    {
        var dataModel = await GetByNameAsync(snapshot.Name, token).ConfigureAwait(false);

        if (dataModel is null)
            throw new CircuitBreakerSnapshotNotFoundException(snapshot.Name);

        _mapper.Map(snapshot, dataModel);

        await _circuitBreakerCollection
            .ReplaceOneAsync(x => x.Name == snapshot.Name, dataModel, cancellationToken: token)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(CircuitBreakerSnapshot snapshot, CancellationToken token)
    {
        var dataModel = _mapper.Map<CircuitBreakerDataModel>(snapshot);
        await _circuitBreakerCollection.InsertOneAsync(dataModel, cancellationToken: token).ConfigureAwait(false);
    }

    private async Task<CircuitBreakerDataModel?> GetByNameAsync(string circuitBreakerName, CancellationToken token)
    {
        var cursor = await _circuitBreakerCollection.FindAsync(
            x => x.Name == circuitBreakerName, cancellationToken: token)
            .ConfigureAwait(false);

        return await cursor.FirstOrDefaultAsync(token).ConfigureAwait(false);
    }
}