using DCB.Core;

namespace DCB.Extensions.SqlServer;

public class SqlServerStore:ICircuitBreakerStore
{
    private readonly CircuitBreakerContext _circuitBreakerContext;

    public SqlServerStore(CircuitBreakerContext circuitBreakerContext)
    {
        _circuitBreakerContext = circuitBreakerContext;
    }
    
    public Task InitializeAsync()
    {
        throw new NotImplementedException();
    }
}