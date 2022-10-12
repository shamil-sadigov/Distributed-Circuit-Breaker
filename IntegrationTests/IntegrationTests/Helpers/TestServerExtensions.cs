using System.Net.Http.Json;
using Core;
using Core.Settings;
using IntegrationTests.WebAppConfiguration.Orders;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Helpers;

public static class TestServerExtensions
{
    public static ICircuitBreaker<TSettings> GetCircuitBreaker<TSettings>(this TestServer server) 
        where TSettings : CircuitBreakerSettings
    {
        return server.Services.GetRequiredService<ICircuitBreaker<TSettings>>();
    }
    
    public static TService GetService<TService>(this TestServer server) where TService : notnull
    {
        return server.Services.GetRequiredService<TService>();
    }
    
    public static void Configure<TService>(this TestServer server, Action<TService> configure) where TService : notnull
    {
        var service =  server.Services.GetRequiredService<TService>();

        configure(service);
    }
    
    public static async Task<HttpResponseMessage> PlaceOrderAsync(this TestServer server, PlaceOrderRequest request)
    {
        var httpClient = server.CreateClient();
        
        return await httpClient.PostAsJsonAsync("orders", request);
    }
    
    
    public static async Task<TResponse?> PlaceOrderAsync<TResponse>(this TestServer server, PlaceOrderRequest request)
    {
        var httpClient = server.CreateClient();
        
        var httpResponse = await httpClient.PostAsJsonAsync("orders", request);
        return await httpResponse.Content.ReadFromJsonAsync<TResponse>();
    }
}