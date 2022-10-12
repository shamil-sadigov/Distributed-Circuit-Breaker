using System.Runtime.CompilerServices;

namespace Helpers;

public static class ThrowingExtensions
{
    public static string ThrowIfNull(
        this string str,
        [CallerArgumentExpression("str")] string argName = "")
    {
        if (string.IsNullOrWhiteSpace(str)) 
            throw new ArgumentNullException(argName);

        return str;
    }

    public static T ThrowIfNull<T>(
        this T obj,
        [CallerArgumentExpression("obj")] string argName = "")
    {
        if (obj is null)
            throw new ArgumentNullException(argName);

        return obj;
    }
}