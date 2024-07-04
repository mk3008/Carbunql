namespace Carbunql;

/// <summary>
/// Represents a query source, which can be a physical table, subquery, or common table expression (CTE).
/// </summary>
public interface IQuerySource
{
    /// <summary>
    /// Gets the branch number of the referenced query source.
    /// Numbering starts from 1.
    /// The query source in the FROM clause inherits the upstream branch number,
    /// while the query source in table joins is assigned a new branch number.
    /// </summary>
    int Branch { get; }

    /// <summary>
    /// Gets the depth level of the query source.
    /// Numbering starts from 1 and increments with each nesting level.
    /// </summary>
    int Level { get; }

    /// <summary>
    /// Gets the sequence number within the select query.
    /// Numbering starts from 1.
    /// </summary>
    int Sequence { get; }

    /// <summary>
    /// Gets the alias name of the query source.
    /// </summary>
    string Alias { get; }

    /// <summary>
    /// Gets the column names belonging to the query source.
    /// Duplicates are removed, but order is not guaranteed.
    /// </summary>
    IEnumerable<string> ColumnNames { get; }

    /// <summary>
    /// Gets the select query to which the query source belongs.
    /// </summary>
    SelectQuery Query { get; }
}
