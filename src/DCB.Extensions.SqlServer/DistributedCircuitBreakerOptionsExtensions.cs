namespace DCB.Extensions.SqlServer;

public static class DistributedCircuitBreakerOptionsExtensions
{
    public static CircuitBreakerContextBuilder UseSqlServer(
        CircuitBreakerContextBuilder builder, 
        string connectionString)
    {
        // configure DBContext here
        
        builder.UseStorage<SqlServerStore>();
        
        return builder;
    }
}