using Carbunql.Analysis.Parser;
using Carbunql.Tables;
using Carbunql.Values;
using Cysharp.Text;

namespace Carbunql.Extensions;

/// <summary>
/// Provides extension methods for the string type.
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// Determines whether the string starts with any of the specified prefixes.
    /// </summary>
    /// <param name="source">The string to check.</param>
    /// <param name="prefixes">The collection of prefixes to check.</param>
    /// <returns>True if the string starts with any of the prefixes; otherwise, false.</returns>
    public static bool StartsWith(this string source, IEnumerable<string> prefixes)
    {
        foreach (var item in prefixes)
        {
            if (source.StartsWith(item)) return true;
        }
        return false;
    }

    /// <summary>
    /// Determines whether the two strings are equal, ignoring case.
    /// </summary>
    /// <param name="source">The first string.</param>
    /// <param name="text">The second string.</param>
    /// <returns>True if the strings are equal, ignoring case; otherwise, false.</returns>
    public static bool IsEqualNoCase(this string? source, string? text)
    {
        if (source == null && text == null) return true;
        if (source == null) return false;
        return string.Equals(source, text, StringComparison.CurrentCultureIgnoreCase);
    }

    /// <summary>
    /// Determines whether the string is equal to any of the specified texts, ignoring case.
    /// </summary>
    /// <param name="source">The string to check.</param>
    /// <param name="texts">The collection of texts to compare against.</param>
    /// <returns>True if the string is equal to any of the specified texts, ignoring case; otherwise, false.</returns>
    public static bool IsEqualNoCase(this string? source, IEnumerable<string?> texts)
    {
        return texts.Where(x => source.IsEqualNoCase(x)).Any();
    }

    /// <summary>
    /// Determines whether the string satisfies the specified predicate, ignoring case.
    /// </summary>
    /// <param name="source">The string to check.</param>
    /// <param name="fn">The predicate to apply to the string.</param>
    /// <returns>True if the string satisfies the predicate, ignoring case; otherwise, false.</returns>
    public static bool IsEqualNoCase(this string? source, Predicate<string?> fn)
    {
        return fn(source);
    }

    /// <summary>
    /// Determines whether the string represents a numeric value.
    /// </summary>
    /// <param name="source">The string to check.</param>
    /// <returns>True if the string represents a numeric value; otherwise, false.</returns>
    public static bool IsNumeric(this string source)
    {
        if (string.IsNullOrEmpty(source)) return false;
        return source.First().IsInteger();
    }

    /// <summary>
    /// Determines whether the string represents an end token.
    /// </summary>
    /// <param name="source">The string to check.</param>
    /// <returns>True if the string represents an end token; otherwise, false.</returns>
    public static bool IsEndToken(this string source)
    {
        if (string.IsNullOrEmpty(source)) return true;
        return (source == ";") ? true : false;
    }

    /// <summary>
    /// Inserts indentation at the beginning of each line in the string.
    /// </summary>
    /// <param name="source">The string to modify.</param>
    /// <param name="separator">The line separator used in the string.</param>
    /// <param name="spaceCount">The number of spaces to indent each line.</param>
    /// <returns>The string with indentation inserted.</returns>
    public static string InsertIndent(this string source, string separator = "\r\n", int spaceCount = 4)
    {
        if (string.IsNullOrEmpty(source)) return source;

        var indent = spaceCount.ToSpaceString();

        return indent + source.Replace(separator, $"{separator}{indent}");
    }

    /// <summary>
    /// Joins the specified items into a single string using the specified separator.
    /// </summary>
    /// <param name="source">The source string.</param>
    /// <param name="jointext">The string to join between each item.</param>
    /// <param name="items">The collection of items to join.</param>
    /// <param name="splitter">The separator used to join the items.</param>
    /// <param name="startDecorate">Optional starting decoration for the joined string.</param>
    /// <param name="endDecorate">Optional ending decoration for the joined string.</param>
    /// <returns>The joined string.</returns>
    public static string Join(this string source, string jointext, IEnumerable<string>? items, string splitter = ",", string? startDecorate = null, string? endDecorate = null)
    {
        if (items == null || !items.Any()) return source;

        using var sb = ZString.CreateStringBuilder();
        sb.Append(source + jointext);
        sb.Append(items.ToString(splitter, startDecorate, endDecorate));
        return sb.ToString();
    }

    /// <summary>
    /// Converts the collection of strings into a single string using the specified separator and decorations.
    /// </summary>
    /// <param name="source">The collection of strings.</param>
    /// <param name="splitter">The separator used between each string.</param>
    /// <param name="startDecorate">Optional starting decoration for the joined string.</param>
    /// <param name="endDecorate">Optional ending decoration for the joined string.</param>
    /// <returns>The joined string.</returns>
    public static string ToString(this IEnumerable<string> source, string splitter = ",", string? startDecorate = null, string? endDecorate = null)
    {
        if (!source.Any()) return string.Empty;

        using var sb = ZString.CreateStringBuilder();
        var isFirst = true;

        if (startDecorate != null) sb.Append(startDecorate);
        foreach (var item in source)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                sb.Append(splitter);
            }
            sb.Append(item);
        }
        if (endDecorate != null) sb.Append(endDecorate);

        return sb.ToString();
    }

    /// <summary>
    /// Converts the string into a PhysicalTable object.
    /// </summary>
    /// <param name="source">The string representing the physical table.</param>
    /// <returns>A PhysicalTable object.</returns>
    public static PhysicalTable ToPhysicalTable(this string source)
    {
        return new PhysicalTable(source);
    }

    /// <summary>
    /// Converts the collection of strings into a ValueCollection object.
    /// </summary>
    /// <param name="source">The collection of strings.</param>
    /// <returns>A ValueCollection object containing the parsed values.</returns>
    public static ValueCollection ToValueCollection(this IEnumerable<string> source)
    {
        var v = new ValueCollection();
        foreach (var item in source) v.Add(ValueParser.Parse(item));
        return v;
    }
}
