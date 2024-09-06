using Carbunql.Clauses;
using Carbunql.Definitions;
using Carbunql.Tables;
using Carbunql.Values;


namespace Carbunql;

/// <summary>
/// Represents a DROP INDEX query.
/// </summary>
public class DropIndexQuery : IAlterIndexQuery
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DropIndexQuery"/> class.
    /// </summary>
    /// <param name="t">The table associated with the index.</param>
    /// <param name="indexName">The name of the index to drop.</param>
    public DropIndexQuery(ITable t, string indexName)
    {
        Schema = t.Schema;
        Table = t.Table;
        IndexName = indexName;
    }

    /// <summary>
    /// Gets or sets the name of the index to drop.
    /// </summary>
    public string IndexName { get; init; }

    /// <summary>
    /// Gets or sets the comment clause.
    /// </summary>
    public CommentClause? CommentClause { get; set; }

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

        yield return Token.Reserved(this, parent, "drop index");
        yield return new Token(this, parent, IndexName);
    }

    /// <summary>
    /// Gets the common tables.
    /// </summary>
    /// <returns>Common tables.</returns>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        yield break;
    }
}
