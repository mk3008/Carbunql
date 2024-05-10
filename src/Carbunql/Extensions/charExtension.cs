namespace Carbunql.Extensions;

/// <summary>
/// Provides extension methods for the char type.
/// </summary>
public static class CharExtension
{
    /// <summary>
    /// Determines whether the specified character is an integer.
    /// </summary>
    /// <param name="source">The character to check.</param>
    /// <returns>True if the character is an integer; otherwise, false.</returns>
    public static bool IsInteger(this char source)
    {
        return "0123456789".Contains(source);
    }
}
