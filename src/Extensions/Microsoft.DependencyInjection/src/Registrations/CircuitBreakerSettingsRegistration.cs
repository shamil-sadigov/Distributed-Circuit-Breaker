using Core.Settings;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq.Extensions;

namespace Extensions.Microsoft.DependencyInjection.Registrations;

public class CircuitBreakerSettingsRegistration
{
    private readonly IServiceCollection _services;
    private readonly HashSet<string> _registeredCircuitBreakerNames = new();
    
    public CircuitBreakerSettingsRegistration(IServiceCollection services)
    {
        _services = services;
    }
    
    public CircuitBreakerSettingsRegistration AddCircuitBreaker<TSettings>()
        where TSettings : CircuitBreakerSettings, new()
    {
        var settings = new TSettings();
        AddCircuitBreakerInternal(settings);
        return this;
    }
    
    public CircuitBreakerSettingsRegistration AddCircuitBreaker<TSettings>(TSettings settings)
        where TSettings : CircuitBreakerSettings
    {
        AddCircuitBreakerInternal(settings);
        return this;
    }
    
    private void AddCircuitBreakerInternal<TSettings>(TSettings settings) where TSettings : CircuitBreakerSettings
    {
        ValidateOptions(settings);
        _services.AddSingleton(settings.GetType(), _ => settings);
    }
    
    private void ValidateOptions(CircuitBreakerSettings settings)
    {
        if (settings is null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        if (!_registeredCircuitBreakerNames.Add(settings.Name))
        {
            throw new CircuitBreakerRegistrationException(
                $"CircuitBreaker with name '{settings.Name}' has already been registered");
        }
        
        var errors = CircuitBreakerSettingsValidator.Validate(settings);

        if (errors.Any())
        {
            var compositeErrorMessage = errors.ToDelimitedString(".");
            throw new CircuitBreakerRegistrationException(compositeErrorMessage);
        }
    }
}