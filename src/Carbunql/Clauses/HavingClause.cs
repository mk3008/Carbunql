using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a HAVING clause in a SQL query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class HavingClause : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HavingClause"/> class with the specified condition.
    /// </summary>
    /// <param name="condition">The condition of the HAVING clause.</param>
    public HavingClause(ValueBase condition)
    {
        Condition = condition;
    }

    /// <summary>
    /// Gets the condition of the HAVING clause.
    /// </summary>
    public ValueBase Condition { get; init; }

    /// <summary>
    /// Gets the internal queries associated with this HAVING clause.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Condition.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the physical tables associated with this HAVING clause.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Condition.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the common tables associated with this HAVING clause.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Condition.GetCommonTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the parameters associated with this HAVING clause.
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        return Condition.GetParameters();
    }

    /// <summary>
    /// Gets the tokens representing this HAVING clause.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "having");
        yield return clause;

        foreach (var item in Condition.GetTokens(clause)) yield return item;
    }
}
