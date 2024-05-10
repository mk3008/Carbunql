using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

/// <summary>
/// Represents a DROP TABLE query.
/// </summary>
public class DropTableQuery : IQueryCommandable, ICommentable, ITable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DropTableQuery"/> class.
    /// </summary>
    /// <param name="t">The table to drop.</param>
    public DropTableQuery(ITable t)
    {
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Gets or sets the comment clause.
    /// </summary>
    [IgnoreMember]
    public CommentClause? CommentClause { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the query should execute if the table exists.
    /// </summary>
    public bool HasIfExists { get; set; } = false;

    /// <summary>
    /// Gets or sets the schema of the table.
    /// </summary>
    public string Schema { get; init; }

    /// <summary>
    /// Gets or sets the name of the table.
    /// </summary>
    public string Table { get; init; }

    /// <summary>
    /// Gets the internal queries.
    /// </summary>
    /// <returns>Internal queries.</returns>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables.
    /// </summary>
    /// <returns>Physical tables.</returns>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    /// <returns>Query parameters.</returns>
    public virtual IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }

    /// <summary>
    /// Gets the tokens.
    /// </summary>
    /// <param name="parent">Parent token.</param>
    /// <returns>Tokens.</returns>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (CommentClause != null) foreach (var item in CommentClause.GetTokens(parent)) yield return item;

        yield return Token.Reserved(this, parent, "drop table");
        if (HasIfExists)
        {
            yield return Token.Reserved(this, parent, "if exists");
        }

        yield return new Token(this, parent, this.GetTableFullName());
    }

    /// <summary>
    /// Gets the common tables.
    /// </summary>
    /// <returns>Common tables.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }
}
