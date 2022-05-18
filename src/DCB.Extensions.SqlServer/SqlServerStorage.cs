using AutoMapper;
using DCB.Core;
using DCB.Core.CircuitBreakers.States;
using DCB.Helpers;
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
        throw new NotImplementedException();
    }

    public Task SaveAsync(CircuitBreakerContextSnapshot snapshot, CancellationToken token)
    {
        throw new NotImplementedException();
    }
    
}