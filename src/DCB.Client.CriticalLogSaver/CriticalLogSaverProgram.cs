using DCB.Client.Shared;
using DCB.Extensions;
using DCB.Extensions.Mongo;

namespace DCB.Client.CriticalLogSaver;

public class CriticalLogSaverProgram
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddDistributedCircuitBreaker(ops =>
        {
            ops.UseMongo(x =>
                {
                    x.ConnectionString = "mongodb://localhost:27017";
                })
                .AddCircuitBreaker<LogStorageCircuitBreakerOptions>();
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