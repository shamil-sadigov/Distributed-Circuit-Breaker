using DCB.Helpers;

namespace DCB.Extensions.Mongo;

public class CircuitBreakerDbOptions
{
    public CircuitBreakerDbOptions(string databaseName, string collectionName)
    {
        databaseName.ThrowIfNull();
        collectionName.ThrowIfNull();

        DatabaseName = databaseName;
        CollectionName = collectionName;
    }

    public string DatabaseName { get; init; }
    public string CollectionName { get; init; }
}