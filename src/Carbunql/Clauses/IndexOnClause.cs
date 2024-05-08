using Carbunql.Definitions;
using Carbunql.Tables;

namespace Carbunql.Clauses;

/// <summary>
/// Represents an INDEX ON clause in a SQL query.
/// </summary>
public class IndexOnClause : QueryCommandCollection<SortableItem>, ITable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IndexOnClause"/> class with the specified table name.
    /// </summary>
    /// <param name="table">The name of the table.</param>
    public IndexOnClause(string table)
    {
        Schema = string.Empty;
        Table = table;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexOnClause"/> class with the specified schema and table names.
    /// </summary>
    /// <param name="schema">The name of the schema.</param>
    /// <param name="table">The name of the table.</param>
    public IndexOnClause(string schema, string table)
    {
        Schema = schema;
        Table = table;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexOnClause"/> class with the specified table.
    /// </summary>
    /// <param name="t">The table.</param>
    public IndexOnClause(ITable t)
    {
        Schema = t.Schema;
        Table = t.Table;
    }

    /// <summary>
    /// Gets or sets the schema name.
    /// </summary>
    public string Schema { get; init; }

    /// <summary>
    /// Gets or sets the table name.
    /// </summary>
    public string Table { get; init; }

    /// <summary>
    /// Gets or sets the USING clause.
    /// </summary>
    public string? Using { get; set; } = null;

    /// <summary>
    /// Gets the tokens representing this INDEX ON clause.
    /// </summary>
    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!Items.Any()) yield break;

        var clause = Token.Reserved(this, parent, "on");
        yield return clause;
        yield return new Token(this, parent, this.GetTableFullName());

        if (!string.IsNullOrEmpty(Using))
        {
            yield return new Token(this, parent, "using", isReserved: true);
            yield return new Token(this, parent, Using);
        }

        yield return Token.ReservedBracketStart(this, parent);
        foreach (var item in base.GetTokens(clause)) yield return item;
        yield return Token.ReservedBracketEnd(this, parent);
    }

    /// <summary>
    /// Gets the internal queries associated with this INDEX ON clause.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <summary>
    /// Gets the physical tables associated with this INDEX ON clause.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the common tables associated with this INDEX ON clause.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }

    /// <summary>
    /// Gets the parameters associated with this INDEX ON clause.
    /// </summary>
    public override IEnumerable<QueryParameter> GetParameters()
    {
        yield break;
    }
}
