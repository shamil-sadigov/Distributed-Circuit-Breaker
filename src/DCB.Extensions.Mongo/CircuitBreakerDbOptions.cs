using System.ComponentModel.DataAnnotations;
using DCB.Shared;

namespace DCB.Extensions.Mongo;

public class CircuitBreakerDbOptions
{
    private string? _connectionString;
    private string _databaseName;
    private string _collectionName;

    public CircuitBreakerDbOptions(string databaseName, string collectionName)
    {
        databaseName.ThrowIfNull();
        collectionName.ThrowIfNull();
        
        _databaseName = databaseName;
        _collectionName = collectionName;
    }

    public string DatabaseName
    {
        get => _databaseName;
        set
        {
            if (value.IsNullOrWhitespace())
                throw new ArgumentNullException(nameof(DatabaseName));

            _databaseName = value;
        }
    }

    public string CollectionName
    {
        get => _collectionName;
        set
        {
            if (value.IsNullOrWhitespace())
                throw new ArgumentNullException(nameof(CollectionName));

            _collectionName = value;
        }
    }

    public string? ConnectionString
    {
        get => _connectionString;
        set
        {
            if (value!.IsNullOrWhitespace())
                throw new ArgumentNullException(nameof(ConnectionString));

            _connectionString = value;
        }
    }
    
    internal void Validate()
    {
        if (CollectionName.IsNullOrWhitespace())
            throw new ValidationException(nameof(CollectionName) + " should have a value");

        if (DatabaseName.IsNullOrWhitespace())
            throw new ValidationException(nameof(DatabaseName) + " should have a value");
        
        if (ConnectionString.IsNullOrWhitespace())
            throw new ValidationException(nameof(ConnectionString) + " should have a value");
    }
}