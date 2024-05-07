namespace Carbunql.Building;

/// <summary>
/// Provides methods for generating <see cref="SelectQuery"/> instances from data items or lists.
/// </summary>
public static class SelectQueryGenerator
{
    /// <summary>
    /// Generates a <see cref="SelectQuery"/> from a single data item.
    /// </summary>
    /// <typeparam name="T">The type of the data item.</typeparam>
    /// <param name="item">The data item.</param>
    /// <param name="parameterSuffix">The suffix for query parameters.</param>
    /// <param name="keyFormatter">A function to format keys.</param>
    /// <returns>A <see cref="SelectQuery"/> based on the data item.</returns>
    public static SelectQuery FromItem<T>(T item, string parameterSuffix = ":", Func<string, string>? keyFormatter = null)
    {
        return FromList(new[] { item }, parameterSuffix, keyFormatter);
    }

    /// <summary>
    /// Generates a <see cref="SelectQuery"/> from a list of data items.
    /// </summary>
    /// <typeparam name="T">The type of the data items.</typeparam>
    /// <param name="list">The list of data items.</param>
    /// <param name="parameterSuffix">The suffix for query parameters.</param>
    /// <param name="keyFormatter">A function to format keys.</param>
    /// <returns>A <see cref="SelectQuery"/> based on the list of data items.</returns>
    public static SelectQuery FromList<T>(IEnumerable<T> list, string parameterSuffix = ":", Func<string, string>? keyFormatter = null)
    {
        keyFormatter ??= (string x) => x;

        var properties = typeof(T).GetProperties().Where(x => x.CanRead).Select(x => x.Name).ToList();
        var valuesQuery = ValuesQueryGenerator.FromList(list, properties, parameterSuffix, keyFormatter);

        var aliases = new List<string>();
        properties.ForEach(x => aliases.Add(keyFormatter(x)));

        return valuesQuery.ToSelectQuery(aliases);
    }
}
