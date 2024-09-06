using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a clause for specifying the number of rows to be returned in a query result.
/// </summary>
public class TopClause : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TopClause"/> class with the specified value.
    /// </summary>
    /// <param name="value">The value representing the number of rows.</param>
    public TopClause(ValueBase value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value representing the number of rows.
    /// </summary>
    public ValueBase Value { get; init; }

    public IEnumerable<ColumnValue> GetColumns()
    {
        return Value.GetColumns();
    }

    /// <summary>
    /// Gets the common tables used in the clause.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        return Value.GetCommonTables();
    }

    /// <summary>
    /// Gets the internal queries used in the clause.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        return Value.GetInternalQueries();
    }

    /// <summary>
    /// Gets the parameters used in the clause.
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        return Value.GetParameters();
    }

    /// <summary>
    /// Gets the physical tables used in the clause.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        return Value.GetPhysicalTables();
    }

    /// <summary>
    /// Gets the tokens representing the clause.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, "top");

        foreach (var item in Value.GetTokens(parent))
        {
            yield return item;
        }
    }
}
