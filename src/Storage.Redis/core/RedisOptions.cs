using System.ComponentModel.DataAnnotations;
using Helpers;

namespace Storage.Redis;

public sealed class RedisOptions
{
    private string _connectionString = null!;
    
    public string ConnectionString
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
        if (ConnectionString.IsNullOrWhitespace())
            throw new ValidationException(nameof(ConnectionString) + " should have a value");
    }
}