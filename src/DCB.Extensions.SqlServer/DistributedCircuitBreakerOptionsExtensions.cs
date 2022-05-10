namespace DCB.Extensions.SqlServer;

public static class DistributedCircuitBreakerOptionsExtensions
{
    public static CircuitBreakerContextBuilder UseSqlServer(
        CircuitBreakerContextBuilder builder, 
        string connectionString)
    {
        // configure DBContext here
        
        builder.UseStorage<SqlServerStorage>();
        
        return builder;
    }
}