using DCB.Core;

namespace DCB.Extensions.SqlServer;

public class SqlServerStorage:ICircuitBreakerStorage
{
    private readonly CircuitBreakerContext _circuitBreakerContext;

    public SqlServerStorage(CircuitBreakerContext circuitBreakerContext)
    {
        _circuitBreakerContext = circuitBreakerContext;
    }
    
    public Task InitializeAsync()
    {
        throw new NotImplementedException();
    }
}