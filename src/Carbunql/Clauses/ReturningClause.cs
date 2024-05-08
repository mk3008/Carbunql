using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a returning clause in a query.
/// </summary>
public class ReturningClause : QueryCommandCollection<ValueBase>, IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReturningClause"/> class with the specified value.
    /// </summary>
    /// <param name="value">The value to be returned.</param>
    public ReturningClause(ValueBase value)
    {
        if (value is ValueCollection collection)
        {
            foreach (var item in collection)
            {
                Items.Add(item);
            }
        }
        else
        {
            Items.Add(value);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturningClause"/> class with the specified values.
    /// </summary>
    /// <param name="values">The values to be returned.</param>
    public ReturningClause(IEnumerable<ValueBase> values)
    {
        foreach (var item in values)
        {
            Items.Add(item);
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        var t = Token.Reserved(this, parent, "returning");
        yield return t;
        foreach (var item in base.GetTokens(t)) yield return item;
    }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        yield break;
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        yield break;
    }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        yield break;
    }
}
