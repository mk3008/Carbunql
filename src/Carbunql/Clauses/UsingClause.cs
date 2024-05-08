using Carbunql.Tables;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a USING clause used in SQL queries.
/// </summary>
public class UsingClause : IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsingClause"/> class with the specified root table, condition, and keys.
    /// </summary>
    /// <param name="root">The root selectable table.</param>
    /// <param name="condition">The condition used in the USING clause.</param>
    /// <param name="keys">The keys used in the USING clause.</param>
    public UsingClause(SelectableTable root, ValueBase condition, IEnumerable<string> keys)
    {
        Root = root;
        Condition = condition;
        Keys = keys;
    }

    /// <summary>
    /// Gets the root selectable table.
    /// </summary>
    public SelectableTable Root { get; init; }

    /// <summary>
    /// Gets the condition used in the USING clause.
    /// </summary>
    public ValueBase Condition { get; init; }

    /// <summary>
    /// Gets the keys used in the USING clause.
    /// </summary>
    public IEnumerable<string> Keys { get; init; }

    /// <summary>
    /// Gets the internal queries used in the clause.
    /// </summary>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Root.GetInternalQueries())
        {
            yield return item;
        }
        foreach (var item in Condition.GetInternalQueries())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the physical tables used in the clause.
    /// </summary>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Root.GetPhysicalTables())
        {
            yield return item;
        }
        foreach (var item in Condition.GetPhysicalTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the common tables used in the clause.
    /// </summary>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Root.GetCommonTables())
        {
            yield return item;
        }
        foreach (var item in Condition.GetCommonTables())
        {
            yield return item;
        }
    }

    /// <summary>
    /// Gets the parameters used in the clause.
    /// </summary>
    public IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var item in Root.GetParameters())
        {
            yield return item;
        }
        if (Condition != null)
        {
            foreach (var item in Condition.GetParameters())
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Gets the tokens representing the USING clause.
    /// </summary>
    public IEnumerable<Token> GetTokens(Token? parent)
    {
        // using
        var t = Token.Reserved(this, parent, "using");
        yield return t;
        foreach (var token in Root.GetTokens(t)) yield return token;

        // on
        foreach (var token in GetOnTokens(t)) yield return token;
    }

    /// <summary>
    /// Gets the tokens representing the 'on' part of the USING clause.
    /// </summary>
    private IEnumerable<Token> GetOnTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, "on");
        foreach (var token in Condition.GetTokens(parent)) yield return token;
    }
}
