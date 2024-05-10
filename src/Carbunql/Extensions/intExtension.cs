using Cysharp.Text;

namespace Carbunql.Extensions;

/// <summary>
/// Provides extension methods for the int type.
/// </summary>
internal static class IntExtension
{
    /// <summary>
    /// Performs the specified action a specified number of times.
    /// </summary>
    /// <param name="source">The number of times to perform the action.</param>
    /// <param name="action">The action to perform.</param>
    public static void ForEach(this int source, Action<int> action)
    {
        for (int i = 0; i < source; i++)
        {
            action(i);
        }
    }

    /// <summary>
    /// Creates a string containing the specified number of space characters.
    /// </summary>
    /// <param name="source">The number of space characters to include in the string.</param>
    /// <returns>A string containing the specified number of space characters.</returns>
    public static string ToSpaceString(this int source)
    {
        using var sb = ZString.CreateStringBuilder();
        source.ForEach(s => sb.Append(" "));
        return sb.ToString();
    }
}
