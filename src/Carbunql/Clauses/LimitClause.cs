using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a clause for limiting the number of rows returned in a SQL query.
/// </summary>
public class LimitClause : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LimitClause"/> class.
    /// </summary>
    public LimitClause()
    {
        Condition = null!;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LimitClause"/> class with the specified text condition.
    /// </summary>
    /// <param name="text">The text condition for limiting the rows.</param>
    public LimitClause(string text)
    {
        Condition = new LiteralValue(text);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LimitClause"/> class with the specified value condition.
    /// </summary>
    /// <param name="item">The value condition for limiting the rows.</param>
    public LimitClause(ValueBase item)
    {
        Condition = item;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LimitClause"/> class with the specified list of conditions.
    /// </summary>
    /// <param name="conditions">The list of conditions for limiting the rows.</param>
    public LimitClause(List<ValueBase> conditions)
    {
        var lst = new ValueCollection();
        conditions.ForEach(x => lst.Add(x));
        Condition = lst;
    }

    /// <summary>
    /// Gets or sets the condition for limiting the rows.
    /// </summary>
    public ValueBase Condition { get; init; }

    /// <summary>
    /// Gets or sets the offset value for skipping rows before starting to return the result set.
    /// </summary>
    public ValueBase? Offset { get; set; }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Condition.GetInternalQueries())
        {
            yield return item;
        }
        if (Offset != null)
        {
            foreach (var item in Offset.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Condition.GetPhysicalTables())
        {
            yield return item;
        }
        if (Offset != null)
        {
            foreach (var item in Offset.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Condition.GetCommonTables())
        {
            yield return item;
        }
        if (Offset != null)
        {
            foreach (var item in Offset.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var item in Condition.GetParameters())
        {
            yield return item;
        }
        var q = Offset?.GetParameters();
        if (q != null)
        {
            foreach (var item in q)
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "limit");
        yield return clause;

        foreach (var item in Condition.GetTokens(clause)) yield return item;
        if (Offset != null)
        {
            yield return Token.Reserved(this, clause, "offset");
            foreach (var item in Offset.GetTokens(clause)) yield return item;
        }
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        foreach (var item in Condition.GetColumns())
        {
            yield return item;
        }
        if (Offset != null)
        {
            foreach (var item in Offset.GetColumns())
            {
                yield return item;
            }
        }
    }
}
