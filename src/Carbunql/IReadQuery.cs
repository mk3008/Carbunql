using Carbunql.Clauses;
using Carbunql.Values;
using MessagePack;

namespace Carbunql;

/// <summary>
/// Represents a query for reading data.
/// </summary>
[Union(0, typeof(SelectQuery))]
[Union(1, typeof(ValuesQuery))]
public interface IReadQuery : IQueryCommandable
{
    /// <summary>
    /// Gets the select clause of the read query.
    /// </summary>
    /// <returns>The select clause if present, otherwise null.</returns>
    SelectClause? GetSelectClause();

    /// <summary>
    /// Gets the with clause of the read query.
    /// </summary>
    /// <returns>The with clause if present, otherwise null.</returns>
    WithClause? GetWithClause();

    /// <summary>
    /// Gets or creates a new <see cref="SelectQuery"/> associated with this read query.
    /// </summary>
    /// <returns>The <see cref="SelectQuery"/>.</returns>
    SelectQuery GetOrNewSelectQuery();

    /// <summary>
    /// Gets the column names involved in the read query.
    /// </summary>
    /// <returns>An enumerable of column names.</returns>
    IEnumerable<string> GetColumnNames();
}

/// <summary>
/// Extension methods for <see cref="IReadQuery"/>.
/// </summary>
public static class IReadQueryExtension
{
    /// <summary>
    /// Converts an <see cref="IReadQuery"/> to a <see cref="ValueBase"/> using a <see cref="QueryContainer"/>.
    /// </summary>
    /// <param name="source">The source <see cref="IReadQuery"/> to convert.</param>
    /// <returns>A <see cref="ValueBase"/> containing the <paramref name="source"/> query.</returns>
    public static ValueBase ToValue(this IReadQuery source)
    {
        return new QueryContainer(source);
    }

    /// <summary>
    /// Creates a deep copy of the <paramref name="source"/> <see cref="IReadQuery"/>.
    /// </summary>
    /// <param name="source">The source <see cref="IReadQuery"/> to copy.</param>
    /// <returns>A deep copy of the <paramref name="source"/> <see cref="IReadQuery"/>.</returns>
    public static IReadQuery DeepCopy(this IReadQuery source)
    {
        var json = MessagePackSerializer.Serialize(source);
        return MessagePackSerializer.Deserialize<IReadQuery>(json);
    }
}
