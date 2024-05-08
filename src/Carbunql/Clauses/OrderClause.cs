using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents an order clause in a SQL query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class OrderClause : QueryCommandCollection<IQueryCommandable>, IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrderClause"/> class.
    /// </summary>
    public OrderClause() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OrderClause"/> class with the specified collection.
    /// </summary>
    /// <param name="collection">The collection of order items.</param>
    public OrderClause(List<IQueryCommandable> collection) : base(collection)
    {
    }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var value in Items)
        {
            foreach (var item in value.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var value in Items)
        {
            foreach (var item in value.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var value in Items)
        {
            foreach (var item in value.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        if (!Items.Any()) yield break;

        var clause = Token.Reserved(this, parent, "order by");
        yield return clause;

        foreach (var item in base.GetTokens(clause)) yield return item;
    }
}
