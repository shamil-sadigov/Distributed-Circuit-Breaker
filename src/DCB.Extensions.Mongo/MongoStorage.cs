using AutoMapper;
using DCB.Core.CircuitBreakers.Context;
using DCB.Core.Exceptions;
using DCB.Core.Storage;
using MongoDB.Driver;

namespace DCB.Extensions.Mongo;

public class MongoStorage : ICircuitBreakerStorage
{
    private readonly IMongoCollection<CircuitBreakerDataModel> _circuitBreakerCollection;
    private readonly IMapper _mapper;

    public MongoStorage(
        MongoClient mongoClient,
        CircuitBreakerDbOptions options,
        IMapper mapper)
    {
        _mapper = mapper;
        _circuitBreakerCollection = mongoClient.GetDatabase(options.DatabaseName)
            .GetCollection<CircuitBreakerDataModel>(options.CollectionName);
    }

    public async Task<CircuitBreakerContextSnapshot?> GetAsync(string circuitBreakerName, CancellationToken token)
    {
        var dataModel = await GetByNameAsync(circuitBreakerName, token);

        return _mapper.Map<CircuitBreakerContextSnapshot>(dataModel);
    }

    public async Task UpdateAsync(CircuitBreakerContextSnapshot snapshot, CancellationToken token)
    {
        var dataModel = await GetByNameAsync(snapshot.Name, token).ConfigureAwait(false);

        if (dataModel is null)
            throw new CircuitBreakerSnapshotNotFoundException(snapshot.Name);

        _mapper.Map(snapshot, dataModel);

        await _circuitBreakerCollection
            .ReplaceOneAsync(x => x.Name == snapshot.Name, dataModel, cancellationToken: token)
            .ConfigureAwait(false);
    }

    public async Task AddAsync(CircuitBreakerContextSnapshot snapshot, CancellationToken token)
    {
        var dataModel = _mapper.Map<CircuitBreakerDataModel>(snapshot);
        await _circuitBreakerCollection.InsertOneAsync(dataModel, cancellationToken: token).ConfigureAwait(false);
    }

    private async Task<CircuitBreakerDataModel?> GetByNameAsync(string circuitBreakerName, CancellationToken token)
    {
        var cursor = await _circuitBreakerCollection.FindAsync(
            x => x.Name == circuitBreakerName,
            cancellationToken: token);

        return await cursor.FirstOrDefaultAsync(token).ConfigureAwait(false);
    }
}