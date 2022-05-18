using DCB.Core.CircuitBreakerOption;
using MoreLinq;

namespace DCB.Extensions.Registry;

/// <summary>
///     TODO: Write unit tests
/// </summary>
public sealed class CircuitBreakerRegistry
{
    private readonly CircuitBreakerOptionsValidator _optionsValidator = new();
    private readonly Dictionary<string, CircuitBreakerOptionsBase> _registry = new();

    public void Add(CircuitBreakerOptionsBase options)
    {
        ValidateOptions(options);

        _registry.Add(options.Name, options);
    }

    public IReadOnlyCollection<CircuitBreakerOptionsBase> GetAll()
    {
        return _registry.Values;
    }

    private void ValidateOptions(CircuitBreakerOptionsBase options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));

        var errors = _optionsValidator.Validate(options);

        if (errors.Any())
        {
            var compositeErrorMessage = errors.ToDelimitedString(".");
            throw new CircuitBreakerOptionsValidationException(compositeErrorMessage);
        }

        if (_registry.ContainsKey(options.Name))
            throw new CircuitBreakerRegistryException(
                $"CircuitBreaker with name '{options.Name}' has already been registered");
    }
}