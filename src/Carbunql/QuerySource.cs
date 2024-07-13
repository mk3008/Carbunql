using Carbunql.Clauses;

namespace Carbunql;

/// <summary>
/// Represents a query source, which can be a physical table, subquery, or common table expression (CTE).
/// This class implements the IQuerySource interface and provides concrete implementations for its properties.
/// </summary>
public class QuerySource(int index, HashSet<string> columnNames, SelectQuery query, SelectableTable source) : IQuerySource
{
    /// <summary>
    /// The index of the query source. It is a unique value within a query, starting from 1.
    /// </summary>
    public int Index { get; } = index;

    /// <summary>
    /// Gets the alias name of the query source.
    /// </summary>
    public string Alias => source.Alias;

    /// <summary>
    /// Gets the column names belonging to the query source.
    /// Duplicates are removed, but order is not guaranteed.
    /// </summary>
    public HashSet<string> ColumnNames => columnNames;

    /// <summary>
    /// Gets the select query to which the query source belongs.
    /// </summary>
    public SelectQuery Query => query;

    /// <summary>
    /// The depth level of the query source. Numbering starts from 1 and increments with each nesting level.
    /// </summary>
    public int MaxLevel => !References.Any() ? 1 : References.Max(x => x.MaxLevel) + 1;

    /// <summary>
    /// The selectable object that contains the query source.
    /// </summary>
    public SelectableTable Source => source;

    public IList<IQuerySource> References { get; } = new List<IQuerySource>();
}

