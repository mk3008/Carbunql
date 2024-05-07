using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Provides methods for generating VALUES queries.
/// </summary>
public static class ValuesQueryGenerator
{
    /// <summary>
    /// Generates a VALUES query from a single item.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="item">The item to generate the query from.</param>
    /// <param name="props">The list of property names to include in the query.</param>
    /// <param name="parameterSufix">Optional. The suffix to append to parameter keys.</param>
    /// <param name="keyFormatter">Optional. A function to format property names into parameter keys.</param>
    /// <returns>A VALUES query generated from the single item.</returns>
    public static ValuesQuery FromItem<T>(T item, List<string> props, string parameterSufix = ":", Func<string, string>? keyFormatter = null)
    {
        return FromList<T>(new[] { item }, props, parameterSufix, keyFormatter);
    }

    /// <summary>
    /// Generates a VALUES query from a collection of items.
    /// </summary>
    /// <typeparam name="T">The type of the items.</typeparam>
    /// <param name="lst">The collection of items to generate the query from.</param>
    /// <param name="props">The list of property names to include in the query.</param>
    /// <param name="parameterSufix">Optional. The suffix to append to parameter keys.</param>
    /// <param name="keyFormatter">Optional. A function to format property names into parameter keys.</param>
    /// <returns>A VALUES query generated from the collection of items.</returns>
    public static ValuesQuery FromList<T>(IEnumerable<T> lst, List<string> props, string parameterSufix = ":", Func<string, string>? keyFormatter = null)
    {
        keyFormatter ??= (string x) => x;

        var rows = new List<ValueCollection>();
        var index = 0;
        foreach (var item in lst)
        {
            if (item == null) continue;
            rows.Add(ValueCollectionGenerator.FromObject(item, props, parameterSufix, keyFormatter, index));
            index++;
        }
        return new ValuesQuery(rows);
    }
}
