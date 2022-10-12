using Extensions.Microsoft.DependencyInjection.Registrations;

namespace Core.Tests.CircuitBreakerTests.Helpers;

public static class CircuitBreakerStorageRegistrationExtensions
{
    public static CircuitBreakerSettingsRegistration UseInMemoryStorage(this CircuitBreakerStorageRegistration storageRegistration)
    {
        return storageRegistration.UseStorage<InMemoryStorage>();
    }
}