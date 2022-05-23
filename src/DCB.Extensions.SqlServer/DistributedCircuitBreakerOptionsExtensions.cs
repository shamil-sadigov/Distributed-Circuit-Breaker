using DCB.Extensions.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DCB.Extensions.SqlServer;

public static class DistributedCircuitBreakerOptionsExtensions
{
    public static CircuitBreakerOptionsBuilder UseSqlServer(
        this CircuitBreakerBuilder builder, 
        Action<CircuitBreakerDbOptions> configureDbOptions)
    {
        var dbOptions = new CircuitBreakerDbOptions();
        configureDbOptions(dbOptions);
        dbOptions.Validate();
        
        builder.Services
            .AddAutoMapper(typeof(DataModelProfile))
            .AddDbContext<CircuitBreakerDbContext>(ops => ops.UseSqlServer(dbOptions.ConnectionString));

        return builder.UseStorage<SqlServerStorage>();
    }
}