using Cysharp.Text;

namespace Carbunql.Extensions;

/// <summary>
/// Provides extension methods for the Token type.
/// </summary>
public static class TokenExtension
{
    /// <summary>
    /// Converts a collection of tokens into a single string.
    /// </summary>
    /// <param name="source">The collection of tokens.</param>
    /// <returns>The string representation of the tokens.</returns>
    public static string ToText(this IEnumerable<Token> source)
    {
        var sb = ZString.CreateStringBuilder();
        Token? prev = null;

        foreach (var item in source)
        {
            if (string.IsNullOrEmpty(item.Text)) continue;
            if (item.NeedsSpace(prev))
            {
                sb.Append(" " + item.Text);
            }
            else
            {
                sb.Append(item.Text);
            }
            prev = item;
        }
        return sb.ToString();
    }
}
