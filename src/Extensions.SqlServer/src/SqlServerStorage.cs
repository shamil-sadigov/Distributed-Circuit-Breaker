using AutoMapper;
using Core.CircuitBreakers.Context;
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

    public async Task<CircuitBreakerState?> GetAsync(string circuitBreakerName, CancellationToken token)
    {
        var dataModel = await _context.CircuitBreakers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == circuitBreakerName, token)
            .ConfigureAwait(false);

        return _mapper.Map<CircuitBreakerState>(dataModel);
    }

    public async Task UpdateAsync(CircuitBreakerState state, CancellationToken token)
    {
        var foundModel = await _context.CircuitBreakers
            .FirstOrDefaultAsync(x => x.Name == state.Name, token)
            .ConfigureAwait(false);

        if (foundModel is null)
            throw new CircuitBreakerSnapshotNotFoundException(state.Name);

        _mapper.Map(state, foundModel);

        await _context.SaveChangesAsync(token).ConfigureAwait(false);
    }

    public async Task AddAsync(CircuitBreakerState state, CancellationToken token)
    {
        var dataModel = _mapper.Map<CircuitBreakerDataModel>(state);

        await _context.CircuitBreakers.AddAsync(dataModel, token).ConfigureAwait(false);

        await _context.SaveChangesAsync(token).ConfigureAwait(false);
    }
}