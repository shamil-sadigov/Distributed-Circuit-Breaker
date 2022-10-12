using Core.Policy;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq.Extensions;

namespace Extensions.Microsoft.DependencyInjection.Registrations;

public class CircuitBreakerPolicyRegistration
{
    private readonly IServiceCollection _services;
    private readonly HashSet<string> _registeredCircuitBreakerNames = new();
    
    public CircuitBreakerPolicyRegistration(IServiceCollection services)
    {
        _services = services;
    }
    
    public CircuitBreakerPolicyRegistration AddCircuitBreaker<TPolicy>()
        where TPolicy : CircuitBreakerPolicy, new()
    {
        var policy = new TPolicy();
        AddCircuitBreakerInternal(policy);
        return this;
    }
    
    public CircuitBreakerPolicyRegistration AddCircuitBreaker<TPolicy>(TPolicy policy)
        where TPolicy : CircuitBreakerPolicy
    {
        AddCircuitBreakerInternal(policy);
        return this;
    }
    
    private void AddCircuitBreakerInternal<TPolicy>(TPolicy policy) where TPolicy : CircuitBreakerPolicy
    {
        ValidateOptions(policy);
        _services.AddSingleton(policy.GetType(), _ => policy);
    }
    
    private void ValidateOptions(CircuitBreakerPolicy policy)
    {
        if (policy is null)
        {
            throw new ArgumentNullException(nameof(policy));
        }

        if (!_registeredCircuitBreakerNames.Add(policy.Name))
        {
            throw new CircuitBreakerRegistrationException(
                $"CircuitBreaker with name '{policy.Name}' has already been registered");
        }
        
        var errors = CircuitBreakerPolicyValidator.Validate(policy);

        if (errors.Any())
        {
            var compositeErrorMessage = errors.ToDelimitedString(".");
            throw new CircuitBreakerRegistrationException(compositeErrorMessage);
        }
    }
}