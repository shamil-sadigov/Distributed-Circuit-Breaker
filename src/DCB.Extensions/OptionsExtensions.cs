namespace DCB.Extensions;

public class OptionsExtensions
{
    private readonly Dictionary<Type, object> _extensions = new();

    public void AddExtension<TExtension>(TExtension extension)
    {
        if (extension is null)
            throw new ArgumentNullException(nameof(extension));

        var extensionType = typeof(TExtension);

        // TODO: Throw Duplicate exception
        if (_extensions.ContainsKey(extensionType))
            throw new ArgumentException(
                nameof(extension),
                $"Extension of type '{_extensions.GetType().Name}' ");

        _extensions.Add(extensionType, extension);
    }
}