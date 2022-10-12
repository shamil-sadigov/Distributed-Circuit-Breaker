using Core.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Extensions.Microsoft.DependencyInjection.Registrations;

public class CircuitBreakerStorageRegistration
{
    private readonly CircuitBreakerPolicyRegistration _policyRegistration;
    public CircuitBreakerStorageRegistration(
        IServiceCollection services, 
        CircuitBreakerPolicyRegistration policyRegistration)
    {
        _policyRegistration = policyRegistration;
        Services = services;
    }

    public IServiceCollection Services { get; }

    public CircuitBreakerPolicyRegistration UseStorage<TStorage>()
        where TStorage : class, ICircuitBreakerStorage
    {
        Services.AddSingleton<ICircuitBreakerStorage, TStorage>();

        return _policyRegistration;
    }
}