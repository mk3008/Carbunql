using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

/// <summary>
/// Represents an ALTER TABLE query.
/// </summary>
public class AlterTableQuery : IQueryCommandable, ICommentable, ITable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlterTableQuery"/> class with the specified ALTER TABLE clause.
    /// </summary>
    /// <param name="clause">The ALTER TABLE clause.</param>
    public AlterTableQuery(AlterTableClause clause)
    {
        AlterTableClause = clause;
    }

    /// <summary>
    /// Gets or sets the ALTER TABLE clause of the query.
    /// </summary>
    public AlterTableClause AlterTableClause { get; set; }

    /// <summary>
    /// Gets or sets the comment clause of the query.
    /// </summary>
    [IgnoreMember]
    public CommentClause? CommentClause { get; set; }

    /// <summary>
    /// Gets the schema of the table.
    /// </summary>
    public string Schema => ((ITable)AlterTableClause).Schema;

    /// <summary>
    /// Gets the name of the table.
    /// </summary>
    public string Table => ((ITable)AlterTableClause).Table;

    /// <summary>
    /// Gets the internal queries.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the tokens of the query.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

        foreach (var item in AlterTableClause.GetTokens(parent))
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the common tables.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters of the query.
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Disassembles the ALTER TABLE query.
    /// </summary>
    public List<AlterTableQuery> Disassemble()
    {
        // 1 command equals 1 query
        var lst = new List<AlterTableQuery>();
        foreach (var item in AlterTableClause)
        {
            lst.Add(new AlterTableQuery(new AlterTableClause(AlterTableClause, item)));
        }
        return lst;
    }

    /// <summary>
    /// Tries to set the table definition clause.
    /// </summary>
    /// <param name="clause">The table definition clause.</param>
    /// <returns><c>true</c> if the table definition clause is successfully set; otherwise, <c>false</c>.</returns>
    public bool TrySet(TableDefinitionClause clause)
    {
        if (AlterTableClause.Count != 1) throw new InvalidOperationException();
        var cmd = AlterTableClause[0];
        return cmd.TrySet(clause);
    }
}
