using AutoMapper;
using Core.CircuitBreakers;
using Core.Exceptions;
using Core.Storage;
using Microsoft.EntityFrameworkCore;

namespace Registration.SqlServer;

public class SqlServerStorage : ICircuitBreakerStorage
{
    private readonly CircuitBreakerDbContext _context;
    private readonly IMapper _mapper;

    public SqlServerStorage(CircuitBreakerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CircuitBreakerSnapshot?> GetAsync(string circuitBreakerName, CancellationToken token)
    {
        var dataModel = await _context.CircuitBreakers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == circuitBreakerName, token)
            .ConfigureAwait(false);

        return _mapper.Map<CircuitBreakerSnapshot>(dataModel);
    }

    public async Task UpdateAsync(CircuitBreakerSnapshot snapshot, CancellationToken token)
    {
        var foundModel = await _context.CircuitBreakers
            .FirstOrDefaultAsync(x => x.Name == snapshot.Name, token)
            .ConfigureAwait(false);

        if (foundModel is null)
            throw new CircuitBreakerSnapshotNotFoundException(snapshot.Name);

        _mapper.Map(snapshot, foundModel);

        await _context.SaveChangesAsync(token).ConfigureAwait(false);
    }

    public async Task AddAsync(CircuitBreakerSnapshot snapshot, CancellationToken token)
    {
        var dataModel = _mapper.Map<CircuitBreakerDataModel>(snapshot);

        await _context.CircuitBreakers.AddAsync(dataModel, token).ConfigureAwait(false);

        await _context.SaveChangesAsync(token).ConfigureAwait(false);
    }
}