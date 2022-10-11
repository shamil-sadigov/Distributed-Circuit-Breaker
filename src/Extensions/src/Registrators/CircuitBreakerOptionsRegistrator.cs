using Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq.Extensions;

namespace Registration.Registrators;

public class CircuitBreakerOptionsRegistrator
{
    private readonly IServiceCollection _services;
    private readonly HashSet<string> _registeredCircuitBreakerNames = new();
    
    public CircuitBreakerOptionsRegistrator(IServiceCollection services)
    {
        _services = services;
    }
    
    public CircuitBreakerOptionsRegistrator AddCircuitBreaker<TOptions>()
        where TOptions : CircuitBreakerSettings, new()
    {
        var options = new TOptions();
        AddCircuitBreakerInternal(options);
        return this;
    }
    
    public CircuitBreakerOptionsRegistrator AddCircuitBreaker<TOptions>(TOptions options)
        where TOptions : CircuitBreakerSettings
    {
        AddCircuitBreakerInternal(options);
        return this;
    }
    
    private void AddCircuitBreakerInternal<TOptions>(TOptions options) where TOptions : CircuitBreakerSettings
    {
        ValidateOptions(options);
        _services.AddSingleton(options.GetType(), _ => options);
    }
    
    private void ValidateOptions(CircuitBreakerSettings options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (!_registeredCircuitBreakerNames.Add(options.Name))
        {
            throw new CircuitBreakerRegistrationException(
                $"CircuitBreaker with name '{options.Name}' has already been registered");
        }
        
        var errors = CircuitBreakerOptionsValidator.Validate(options);

        if (errors.Any())
        {
            var compositeErrorMessage = errors.ToDelimitedString(".");
            throw new CircuitBreakerRegistrationException(compositeErrorMessage);
        }
    }
}