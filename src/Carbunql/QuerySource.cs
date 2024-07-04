namespace Carbunql;

/// <summary>
/// Query source
/// </summary>
/// <param name="branch">The branch number of the referenced query source. Numbering starts from 1.</param>
/// <param name="level">The depth level of the query source. Numbering starts from 1 and increments with each nesting level.</param>
/// <param name="sequence">The sequence number within the select query. Numbering starts from 1.</param>
/// <param name="alias">The alias name of the query source.</param>
/// <param name="columnNames">The column names belonging to the query source. Duplicates are removed, but order is not guaranteed.</param>
/// <param name="query">The select query to which the query source belongs.</param>
public class QuerySource(int branch, int level, int sequence, string alias, IEnumerable<string> columnNames, SelectQuery query) : IQuerySource
{
    /// <summary>
    /// The branch number of the referenced query source.
    /// Numbering starts from 1.
    /// The query source in the FROM clause inherits the upstream branch number,
    /// while the query source in table joins is assigned a new branch number.
    /// </summary>
    public int Branch { get; } = branch;

    /// <summary>
    /// The alias name of the query source.
    /// </summary>
    public string Alias => alias;

    /// <summary>
    /// The column names belonging to the query source. Duplicates are removed, but order is not guaranteed.
    /// </summary>
    public IEnumerable<string> ColumnNames => columnNames;

    /// <summary>
    /// The select query to which the query source belongs.
    /// </summary>
    public SelectQuery Query => query;

    /// <summary>
    /// The depth level of the query source. Numbering starts from 1 and increments with each nesting level.
    /// </summary>
    public int Level { get; } = level;

    /// <summary>
    /// The sequence number within the select query. Numbering starts from 1.
    /// </summary>
    public int Sequence { get; } = sequence;
}
