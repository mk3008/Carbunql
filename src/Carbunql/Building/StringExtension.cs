using Carbunql.Analysis.Parser;
using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Provides extension methods for <see cref="string"/> objects.
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// Converts a list of strings to a <see cref="ValueCollection"/>.
    /// </summary>
    /// <param name="source">The list of strings.</param>
    /// <returns>A <see cref="ValueCollection"/> containing parsed values from the list.</returns>
    public static ValueCollection ToValueCollection(this IList<string> source)
    {
        var values = new ValueCollection();
        foreach (var item in source)
        {
            values.Add(ValueParser.Parse(item));
        }
        return values;
    }

    /// <summary>
    /// Converts a list of strings to a <see cref="ValueCollection"/> with a specified alias.
    /// </summary>
    /// <param name="source">The list of strings.</param>
    /// <param name="alias">The alias for the column values.</param>
    /// <returns>A <see cref="ValueCollection"/> containing column values with the specified alias.</returns>
    public static ValueCollection ToValueCollection(this IList<string> source, string alias)
    {
        var values = new ValueCollection();
        foreach (var item in source)
        {
            values.Add(new ColumnValue(alias, item));
        }
        return values;
    }
}
