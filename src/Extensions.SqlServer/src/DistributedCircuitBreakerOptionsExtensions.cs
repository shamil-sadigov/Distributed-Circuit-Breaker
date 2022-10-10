using Registration.Registrators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Registration.SqlServer;

public static class DistributedCircuitBreakerOptionsExtensions
{
    public static CircuitBreakerOptionsRegistrator UseSqlServer(
        this CircuitBreakerStorageRegistrator storageRegistrator, 
        Action<CircuitBreakerDbOptions> configureDbOptions)
    {
        var dbOptions = new CircuitBreakerDbOptions();
        configureDbOptions(dbOptions);
        dbOptions.Validate();
        
        storageRegistrator.Services
            .AddAutoMapper(typeof(DataModelProfile))
            .AddDbContext<CircuitBreakerDbContext>(ops => ops.UseSqlServer(dbOptions.ConnectionString));

        return storageRegistrator.UseStorage<SqlServerStorage>();
    }
}