using Helpers;
using Microsoft.Extensions.Options;

namespace Registration.Mongo;

public class ValidateCircuitBreakerDbOptions:IValidateOptions<MongoDbOptions>
{
    public ValidateOptionsResult Validate(string name, MongoDbOptions options)
    {
        var errors = Validate(options);

        if (errors.Count == 0)
            return ValidateOptionsResult.Success;

        return ValidateOptionsResult.Fail(errors);
    }

    private static List<string> Validate(MongoDbOptions options)
    {
        var errors = new List<string>(3);

        if (options.CollectionName.IsNullOrWhitespace())
            errors.Add(nameof(options.CollectionName) + " should have a value");

        if (options.DatabaseName.IsNullOrWhitespace())
            errors.Add(nameof(options.DatabaseName) + " should have a value");

        if (options.ConnectionString.IsNullOrWhitespace())
            errors.Add(nameof(options.ConnectionString) + " should have a value");
        return errors;
    }
}