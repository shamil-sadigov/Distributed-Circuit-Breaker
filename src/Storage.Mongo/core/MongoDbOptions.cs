using System.ComponentModel.DataAnnotations;
using Helpers;

namespace Storage.Mongo;

public sealed class MongoDbOptions
{
    private string? _connectionString;
    private string _databaseName;
    private string _collectionName;

    public MongoDbOptions(string databaseName, string collectionName)
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
    
    public void Validate()
    {
        if (CollectionName.IsNullOrWhitespace())
            throw new ValidationException(nameof(CollectionName) + " should have a value");

        if (DatabaseName.IsNullOrWhitespace())
            throw new ValidationException(nameof(DatabaseName) + " should have a value");
        
        if (ConnectionString.IsNullOrWhitespace())
            throw new ValidationException(nameof(ConnectionString) + " should have a value");
    }
}