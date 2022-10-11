using System.ComponentModel.DataAnnotations;
using Helpers;

namespace Storage.SqlServer;

public class CircuitBreakerDbOptions
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
            throw new ValidationException("Connection string should be provided");
    }
}