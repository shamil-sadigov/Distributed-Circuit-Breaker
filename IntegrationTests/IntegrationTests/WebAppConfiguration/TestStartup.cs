using Extensions.Microsoft.DependencyInjection;
using IntegrationTests.WebAppConfiguration.Shipment;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Storage.Redis;
using Storage.Redis.Extensions.Microsoft.DependencyInjection;

namespace IntegrationTests.WebAppConfiguration;

public class TestStartup : StartupBase
{
    private readonly RedisOptions _redisOptions;

    public TestStartup(RedisOptions redisOptions)
    {
        _redisOptions = redisOptions;
    }
    
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddDistributedCircuitBreaker(ops => ops
                .UseRedis(_redisOptions)
                .AddCircuitBreaker<ShipmentServiceSettings>())
        .AddSingleton<ShipmentService>()
        .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy())
        .Services
        .AddControllers();
    }
    
    public override void Configure(IApplicationBuilder app)
    {
        app.UseHealthChecks("/health");
        app.UseRouting();
        app.UseEndpoints(x=> x.MapControllers());
    }
}
