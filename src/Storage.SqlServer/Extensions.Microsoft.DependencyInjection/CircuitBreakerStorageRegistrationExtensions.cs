using Extensions.Microsoft.DependencyInjection.Registrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Storage.SqlServer.Extensions.Microsoft.DependencyInjection;

public static class DistributedCircuitBreakerOptionsExtensions
{
    public static CircuitBreakerSettingsRegistration UseSqlServer(
        this CircuitBreakerStorageRegistration storageRegistration, 
        Action<CircuitBreakerDbOptions> configureDbOptions)
    {
        var dbOptions = new CircuitBreakerDbOptions();
        configureDbOptions(dbOptions);
        dbOptions.Validate();
        
        storageRegistration.Services
            .AddAutoMapper(typeof(DataModelProfile))
            .AddDbContext<CircuitBreakerDbContext>(ops => ops.UseSqlServer(dbOptions.ConnectionString));

        return storageRegistration.UseStorage<SqlServerStorage>();
    }
}