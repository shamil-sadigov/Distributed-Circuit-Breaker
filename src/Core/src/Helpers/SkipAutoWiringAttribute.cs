namespace Core.Helpers;

/// <summary>
/// Just a marker attribute indicating than there is no need to register decorated object in DI container
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SkipAutoWiringAttribute : Attribute
{
    
}