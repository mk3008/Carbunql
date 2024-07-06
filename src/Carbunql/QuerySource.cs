using Carbunql.Clauses;

namespace Carbunql;

/// <summary>
/// Represents a query source, which can be a physical table, subquery, or common table expression (CTE).
/// This class implements the IQuerySource interface and provides concrete implementations for its properties.
/// </summary>
public class QuerySource(int parentBranch, HashSet<int> indexes, int level, int sequence, HashSet<string> columnNames, SelectQuery query, SelectableTable source) : IQuerySource
{
    /// <summary>
    /// The ID of the parent query source. If it doesn't exist, it's 0.
    /// </summary>
    public int ParentIndex { get; } = parentBranch;

    /// <summary>
    /// The index of the query source. It is a unique value within a query, starting from 1.
    /// </summary>
    public int SourceIndex { get; } = indexes.Last();

    /// <summary>
    /// Indicates the order in which the query source is referenced.
    /// </summary>
    public HashSet<int> ReferencedIndexes { get; } = indexes;

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
    public int Level { get; } = level;

    /// <summary>
    /// Gets the sequence number within the select query.
    /// Numbering starts from 1.
    /// </summary>
    public int Sequence { get; } = sequence;

    /// <summary>
    /// The selectable object that contains the query source.
    /// </summary>
    public SelectableTable Source => source;
}
