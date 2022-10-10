using AutoMapper;
using Core.CircuitBreakers.Context;
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

    public async Task<CircuitBreakerState?> GetAsync(string circuitBreakerName, CancellationToken token)
    {
        var dataModel = await GetByNameAsync(circuitBreakerName, token).ConfigureAwait(false);

        return _mapper.Map<CircuitBreakerState>(dataModel);
    }

    public async Task UpdateAsync(CircuitBreakerState state, CancellationToken token)
    {
        var dataModel = await GetByNameAsync(state.Name, token).ConfigureAwait(false);

        if (dataModel is null)
            throw new CircuitBreakerSnapshotNotFoundException(state.Name);

        _mapper.Map(state, dataModel);

        await _circuitBreakerCollection
            .ReplaceOneAsync(x => x.Name == state.Name, dataModel, cancellationToken: token)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(CircuitBreakerState state, CancellationToken token)
    {
        var dataModel = _mapper.Map<CircuitBreakerDataModel>(state);
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