using DCB.Extensions.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DCB.Extensions.SqlServer;

public static class DistributedCircuitBreakerOptionsExtensions
{
    public static CircuitBreakerOptionsBuilder UseSqlServer(
        CircuitBreakerBuilder builder, 
        string connectionString)
    {
        builder.Services.AddDbContext<CircuitBreakerDbContext>(ops =>
        {
            ops.UseSqlServer(connectionString);
        });

        builder.Services.AddAutoMapper(typeof(DataModelMapper));
        
       return builder.UseStorage<SqlServerStorage>();
    }
}