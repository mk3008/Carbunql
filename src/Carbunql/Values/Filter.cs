using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

/// <summary>
/// Represents a FILTER clause.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class Filter : IQueryCommandable
{
    /// <summary>
    /// Gets or sets the WHERE clause.
    /// </summary>
    public WhereClause? WhereClause { get; set; }

    public IEnumerable<ColumnValue> GetColumns()
    {
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetColumns())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<QueryParameter> GetParameters()
    {
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetParameters())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (WhereClause == null) yield break;

        var filterToken = Token.Reserved(this, parent, "filter");
        yield return filterToken;

        var bracket = Token.ReservedBracketStart(this, filterToken);
        yield return bracket;
        if (WhereClause != null)
        {
            foreach (var item in WhereClause.GetTokens(bracket)) yield return item;
        }
        yield return Token.ReservedBracketEnd(this, filterToken);
    }
}
