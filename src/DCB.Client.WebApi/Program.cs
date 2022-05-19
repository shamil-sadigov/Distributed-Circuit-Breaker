using DCB.Client.WebApi;
using DCB.Client.WebApi.CircuitBreakerOptions;
using DCB.Extensions;
using DCB.Extensions.Mongo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDistributedCircuitBreaker(ops =>
{
    ops.UseMongo("mongodb://localhost:27017")
        .AddCircuitBreaker<EventStoreCircuitBreakerOptions>()
        .AddCircuitBreaker<AlertServiceCircuitBreakerOptions>();
});

builder.Services.AddSingleton<EventStore>();

var app = builder.Build();

app.MapControllers();
app.Run();