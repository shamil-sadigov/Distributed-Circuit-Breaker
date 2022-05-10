using System.Runtime.CompilerServices;

namespace DCB.Helpers;

public static class ThrowingExtensions
{
    public static void ThrowIfNull(
        this string str,
        [CallerArgumentExpression("str")] string argName = "")
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentNullException(argName);
        }
    }
    
    public static void ThrowIfNull<T>(
        this T obj,
        [CallerArgumentExpression("obj")] string argName = "")
    {
        if (obj is null)
        {
            throw new ArgumentNullException(argName);
        }
    }
    
    public static void ThrowIfLessThan(
        this int value,
        int value2,
        [CallerArgumentExpression("value")] string argName = "")
    {
        if (value < value2)
        {
            throw new ArgumentOutOfRangeException(argName);
        }
    }
    
    public static void ThrowIfLessOrEqualThan(
        this int value,
        int value2,
        [CallerArgumentExpression("value")] string argName = "")
    {
        if (value <= value2)
        {
            throw new ArgumentOutOfRangeException(argName);
        }
    }
    
    
    public static void ThrowIfLessThan(
        this TimeSpan value,
        TimeSpan value2,
        [CallerArgumentExpression("value")] string argName = "")
    {
        if (value < value2)
        {
            throw new ArgumentOutOfRangeException(argName);
        }
    }
    
    public static void ThrowIfLessOrEqualThan(
        this TimeSpan value,
        TimeSpan value2,
        [CallerArgumentExpression("value")] string argName = "")
    {
        if (value <= value2)
        {
            throw new ArgumentOutOfRangeException(argName);
        }
    }
}