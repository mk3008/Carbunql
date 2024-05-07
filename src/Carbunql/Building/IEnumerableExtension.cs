using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql.Building;

/// <summary>
/// Provides extension methods for working with <see cref="IEnumerable{T}"/> collections.
/// </summary>
public static class IEnumerableExtension
{
    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> collection to a <see cref="ValuesQuery"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <returns>A <see cref="ValuesQuery"/> representing the data in the collection.</returns>
    public static ValuesQuery ToValuesQuery<T>(this IEnumerable<T> source)
    {
        return ToValuesQuery(source, DbmsConfiguration.PlaceholderIdentifier);
    }

    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> collection to a <see cref="ValuesQuery"/> with a specified placeholder identifier.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="placeholderIndentifer">The placeholder identifier.</param>
    /// <returns>A <see cref="ValuesQuery"/> representing the data in the collection.</returns>
    public static ValuesQuery ToValuesQuery<T>(this IEnumerable<T> source, string placeholderIndentifer)
    {
        var vq = new ValuesQuery();
        var r = 0;
        foreach (var row in source)
        {
            var lst = new List<ValueBase>();
            var c = 0;
            foreach (var column in typeof(T).GetProperties().Where(x => x.CanRead).Select(x => x.GetValue(row)))
            {
                var name = $"r{r}c{c}";
                var v = placeholderIndentifer + name;
                lst.Add(ValueParser.Parse(v));
                vq.AddParameter(name, column);
                c++;
            }
            vq.Rows.Add(new ValueCollection(lst));
            r++;
        }
        return vq;
    }

    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> collection to a <see cref="SelectQuery"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <returns>A <see cref="SelectQuery"/> representing the data in the collection.</returns>
    public static SelectQuery ToSelectQuery<T>(this IEnumerable<T> source)
    {
        return source.ToSelectQuery(DbmsConfiguration.PlaceholderIdentifier, x => x.ToLowerSnakeCase());
    }

    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> collection to a <see cref="SelectQuery"/> with a specified placeholder identifier and property name converter.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="placeholderIndentifer">The placeholder identifier.</param>
    /// <param name="propertyNameConverter">A function to convert property names.</param>
    /// <returns>A <see cref="SelectQuery"/> representing the data in the collection.</returns>
    public static SelectQuery ToSelectQuery<T>(this IEnumerable<T> source, string placeholderIndentifer, Func<string, string> propertyNameConverter)
    {
        var columns = typeof(T).GetProperties().Where(x => x.CanRead).Select(x => propertyNameConverter(x.Name));
        var vq = source.ToValuesQuery(placeholderIndentifer);
        return vq.ToSelectQuery(columns);
    }
}
