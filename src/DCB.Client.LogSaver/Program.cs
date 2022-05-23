using DCB.Client.WebApi.CircuitBreakerOptions;
using DCB.Extensions;
using DCB.Extensions.Mongo;

namespace DCB.Client.WebApi;

public class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions());
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.Services.AddDistributedCircuitBreaker(ops =>
        {
            ops.UseMongo(x =>
                {
                    x.ConnectionString = "mongodb://localhost:27017";
                })
                .AddCircuitBreaker<EventStoreCircuitBreakerOptions>()
                .AddCircuitBreaker<AlertServiceCircuitBreakerOptions>();
        });

        builder.Services.AddSingleton<LogStorage>();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.MapControllers();
        app.Run();
    }
}