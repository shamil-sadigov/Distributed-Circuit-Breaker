using System.Net;
using FluentAssertions;
using FunctionalTests.Helpers;
using FunctionalTests.WebAppConfiguration.Orders;
using FunctionalTests.WebAppConfiguration.Shipment;
using FunctionalTests.WebAppConfiguration.Shipment.ShipmentStrategies;
using Microsoft.AspNetCore.TestHost;
using MoreLinq;
using MoreLinq.Experimental;
using Xunit;

namespace FunctionalTests;

// TODO: Add tests over HalfOpen state scenarios

public class CircuitBreakerTests:IClassFixture<ServerFactory>
{
    private readonly ServerFactory _serverFactory;

    public CircuitBreakerTests(ServerFactory serverFactory)
    {
        _serverFactory = serverFactory;
    }
    
    [Fact]
    public async Task CircuitBreaker_remains_closed_when_shipment_service_is_available_during_order_placement()
    {
        // Arrange
        // Let's say we have 5 instances of the same service
        TestServer[] serverReplicas = CreateServerReplicas(numberOfReplicas: 5)
            .Pipe(server =>
            {
                server.Configure<ShipmentService>(x => x.ShipmentStrategy = new SuccessfulShipmentStrategy());
            }).ToArray();
        
        // Act & Assert
        foreach (var server in RoundRobin(serverReplicas).Take(50)) // emulate 50 requests across 5 replicas
        {
            var response = await server.PlaceOrderAsync<PlaceOrderResponse>(new PlaceOrderRequest()
            {
                OrderId = "SomeId"
            });

            response!.Succeeded.Should().BeTrue();
        }
        
         // CircuitBreaker on all servers is closed
         serverReplicas
             .Select(server => server.GetCircuitBreaker<ShipmentServicePolicy>())
             .Pipe(circuitBreaker =>
             {
                 circuitBreaker.IsClosed()
                     .Should().BeTrue("Because shipment service was available");

                 circuitBreaker.GetFailedTimes()
                     .Should().Be(0);
             });
    }
    
    [Fact]
    public async Task CircuitBreaker_become_opened_when_shipment_service_is_unavailable_during_order_placement()
    {
        // Arrange
        // Let's say we have 5 instances of the same service
        TestServer[] serverReplicas = CreateServerReplicas(numberOfReplicas: 5)
            .Pipe(server =>
            {
                server.Configure<ShipmentService>(x => x.ShipmentStrategy = new ServiceUnavailableStrategy());
                server.Configure<ShipmentServicePolicy>(x => x.FailureAllowed = 10);
            })
            .ToArray();
        
        // Act & Assert
        
        foreach (var server in RoundRobin(serverReplicas).Take(10)) // emulate 10 failed requests across 5 replicas
        {
            HttpResponseMessage response = await server.PlaceOrderAsync(new PlaceOrderRequest()
            {
                OrderId = "SomeId"
            });
            
            response.ShouldBe(HttpStatusCode.ServiceUnavailable);
        }
        
        serverReplicas
            .Select(server => server.GetCircuitBreaker<ShipmentServicePolicy>())
            .Pipe(circuitBreaker =>
            {
                circuitBreaker.IsOpen().Should().BeTrue(
                        "Because CircuitBreaker handles cases when shipment service is unavailable, " +
                        "and eventually (after 10 failures) becomes opened");
                
                circuitBreaker.GetFailedTimes()
                    .Should().Be(10);
            });
    }
    
    
    [Fact]
    public async Task CircuitBreaker_remains_closed_event_when_shipment_service_rate_is_limited_during_order_creation()
    {
        // Arrange
        // Let's say we have 5 instances of the same service
        TestServer[] serverReplicas = CreateServerReplicas(numberOfReplicas: 5)
            .Pipe(server =>
            {
                server.Configure<ShipmentService>(x => x.ShipmentStrategy = new RateLimitedStrategy());
                server.Configure<ShipmentServicePolicy>(x => x.FailureAllowed = 10);
            })
            .ToArray();
        
        // Act & Assert
        
        foreach (var server in RoundRobin(serverReplicas).Take(20))
        {
            HttpResponseMessage response = await server.PlaceOrderAsync(new PlaceOrderRequest()
            {
                OrderId = "SomeId"
            });
            
            response.ShouldBe(HttpStatusCode.ServiceUnavailable);
        }
        
        serverReplicas
            .Select(server => server.GetCircuitBreaker<ShipmentServicePolicy>())
            .Pipe(circuitBreaker =>
            {
                circuitBreaker.IsClosed().Should().BeTrue(
                    "Because failures relate to shipment service rate limitation is not handled by CircuitBreaker");
                
                circuitBreaker.GetFailedTimes()
                    .Should().Be(0);
            });
    }
    
    
    // Hmmm... What a name....
    [Fact]
    public async Task When_allowed_number_of_failures_is_not_reached_CircuitBreaker_remains_closed_when_shipment_service_is_unavailable_during_order_creation()
    {
        // Arrange
        // Let's say we have 5 instances of the same service
        TestServer[] serverReplicas = CreateServerReplicas(numberOfReplicas: 5)
            .Pipe(server =>
            {
                server.Configure<ShipmentService>(x => x.ShipmentStrategy = new ServiceUnavailableStrategy());
                server.Configure<ShipmentServicePolicy>(x => x.FailureAllowed = 10);
            })
            .ToArray();
        
        // Act & Assert
        
        foreach (var server in RoundRobin(serverReplicas).Take(8)) // emulate 8 failed requests
        {
            HttpResponseMessage response = await server.PlaceOrderAsync(new PlaceOrderRequest()
            {
                OrderId = "SomeId"
            });
            
            response.ShouldBe(HttpStatusCode.ServiceUnavailable);
        }
        
        serverReplicas
            .Select(x => x.GetCircuitBreaker<ShipmentServicePolicy>())
            .Pipe(circuitBreaker =>
            {
                circuitBreaker.IsClosed().Should().BeTrue(
                    "Because 10 number of failures is not reached yet");
                
                circuitBreaker.GetFailedTimes()
                    .Should().Be(8);
            });
    }
    
    private static IEnumerable<TestServer> RoundRobin(params TestServer[] testServers)
    {
        while (true)
            foreach (var server in testServers)
                yield return server;
    }
    
    private IEnumerable<TestServer> CreateServerReplicas(int numberOfReplicas) =>
        Enumerable.Range(0, numberOfReplicas)
            .Select(async _ => await _serverFactory.CreateRunningServerAsync())
            .Await();
}