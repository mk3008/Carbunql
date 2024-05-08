using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a clause for the "INSERT" part of a "MERGE" SQL statement.
/// </summary>
public class MergeInsertClause : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MergeInsertClause"/> class with the specified column aliases.
    /// </summary>
    /// <param name="columnAliases">The column aliases.</param>
    public MergeInsertClause(ValueCollection columnAliases)
    {
        ColumnAliases = columnAliases;
    }

    /// <summary>
    /// Gets the column aliases.
    /// </summary>
    public ValueCollection ColumnAliases { get; init; }

    /// <inheritdoc/>
    public virtual IEnumerable<Token> GetTokens(Token? parent)
    {
        var t = Token.Reserved(this, parent, "insert");

        var bracket = Token.ReservedBracketStart(this, t);
        yield return bracket;
        foreach (var item in ColumnAliases.GetTokens(bracket)) yield return item;
        yield return Token.ReservedBracketEnd(this, parent);
    }

    /// <inheritdoc/>
    public virtual IEnumerable<QueryParameter> GetParameters()
    {
        return ColumnAliases.GetParameters();
    }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in ColumnAliases.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in ColumnAliases.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in ColumnAliases.GetCommonTables())
        {
            yield return item;
        }
    }
}
