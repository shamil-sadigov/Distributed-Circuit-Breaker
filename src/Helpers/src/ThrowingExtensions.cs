using System.Runtime.CompilerServices;

namespace Helpers;

public static class ThrowingExtensions
{
    public static string ThrowIfNull(
        this string str,
        [CallerArgumentExpression("str")] string argName = "")
    {
        if (string.IsNullOrWhiteSpace(str)) throw new ArgumentNullException(argName);

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

    public static void ThrowIfDefault(
        this DateTime value,
        [CallerArgumentExpression("value")] string argName = "")
    {
        if (value == default) throw new ArgumentException("Should not have default value", argName);
    }

    public static void ThrowIfDefault(
        this TimeSpan value,
        [CallerArgumentExpression("value")] string argName = "")
    {
        if (value == default) throw new ArgumentException("Should not have default value", argName);
    }

    public static void ThrowIfGreaterOrEqualTo(
        this int value,
        int value2,
        [CallerArgumentExpression("value")] string valueName = "",
        [CallerArgumentExpression("value2")] string value2Name = "")
    {
        if (value >= value2)
            throw new ArgumentException($"Expected to be less than {value2Name}", valueName);
    }

    public static void ThrowIfLessThan(
        this int value,
        int value2,
        [CallerArgumentExpression("value")] string valueName = "",
        [CallerArgumentExpression("value2")] string value2Name = "")
    {
        if (value < value2)
            throw new ArgumentException($"Expected to be greater than {value2Name}", valueName);
    }

    public static void ThrowIfLessOrEqualTo(
        this int value,
        int value2,
        [CallerArgumentExpression("value")] string valueName = "",
        [CallerArgumentExpression("value2")] string value2Name = "")
    {
        if (value <= value2)
            throw new ArgumentException($"Expected to be greater than or equal to {value2Name}", valueName);
    }

    public static void ThrowIfLessThan(
        this TimeSpan value,
        TimeSpan value2,
        [CallerArgumentExpression("value")] string valueName = "")
    {
        if (value < value2) throw new ArgumentOutOfRangeException(valueName);
    }

    public static void ThrowIfLessOrEqualTo(
        this TimeSpan value,
        TimeSpan value2,
        [CallerArgumentExpression("value")] string argName = "")
    {
        if (value <= value2) throw new ArgumentOutOfRangeException(argName);
    }
}