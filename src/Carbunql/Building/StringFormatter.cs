using Cysharp.Text;

namespace Carbunql.Building;

/// <summary>
/// Provides extension methods for string formatting.
/// </summary>
public static class StringFormatter
{
    private static readonly char[] UpperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    /// <summary>
    /// Converts a string to upper snake case.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <returns>The string converted to upper snake case.</returns>
    public static string ToUpperSnakeCase(this string value) => ToSnakeCase(value).ToUpper();

    /// <summary>
    /// Converts a string to lower snake case.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <returns>The string converted to lower snake case.</returns>
    public static string ToLowerSnakeCase(this string value) => ToSnakeCase(value).ToLower();

    /// <summary>
    /// Converts a string to snake case.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <returns>The string converted to snake case.</returns>
    public static string ToSnakeCase(this string value)
    {
        using var sb = ZString.CreateStringBuilder();
        var isPrevUpper = false;

        foreach (var item in value)
        {
            var isUpper = UpperChars.Contains(item);
            if (sb.Length == 0)
            {
                sb.Append(item);
            }
            else if (isUpper && !isPrevUpper)
            {
                sb.Append("_");
                sb.Append(item);
            }
            else
            {
                sb.Append(item);
            }
            isPrevUpper = isUpper;
        }
        return sb.ToString();
    }
}
