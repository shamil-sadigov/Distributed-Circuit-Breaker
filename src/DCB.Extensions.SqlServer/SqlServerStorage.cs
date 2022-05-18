using AutoMapper;
using DCB.Core.CircuitBreakers.Context;
using DCB.Core.Exceptions;
using DCB.Core.Storage;
using Microsoft.EntityFrameworkCore;

namespace DCB.Extensions.SqlServer;

public class SqlServerStorage:ICircuitBreakerStorage
{
    private readonly CircuitBreakerDbContext _context;
    private readonly IMapper _mapper;

    public SqlServerStorage(CircuitBreakerDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<CircuitBreakerContextSnapshot?> GetAsync(string circuitBreakerName, CancellationToken token)
    {
        var dataModel = await _context.CircuitBreakers
            .AsNoTracking()
            .FirstOrDefaultAsync(x=> x.Name == circuitBreakerName, token);
        
        return _mapper.Map<CircuitBreakerContextSnapshot>(dataModel);
    }

    public async Task UpdateAsync(CircuitBreakerContextSnapshot snapshot, CancellationToken token)
    {
        var foundModel = await _context.CircuitBreakers.FirstOrDefaultAsync(x=> x.Name == snapshot.Name, token);

        if (foundModel is null)
            throw new CircuitBreakerSnapshotNotFoundException(snapshot.Name);

        _mapper.Map(snapshot, foundModel);
        
        await _context.SaveChangesAsync(token);
    }

    public async Task AddAsync(CircuitBreakerContextSnapshot snapshot, CancellationToken token)
    {
        var dataModel = _mapper.Map<CircuitBreakerDataModel>(snapshot);

        await _context.CircuitBreakers.AddAsync(dataModel, token);

        await _context.SaveChangesAsync(token);
    }
}