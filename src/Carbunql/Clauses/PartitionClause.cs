using Carbunql.Tables;
using Carbunql.Values;
using MessagePack;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a PARTITION BY clause in a SQL query.
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class PartitionClause : QueryCommandCollection<ValueBase>, IQueryCommandable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PartitionClause"/> class.
    /// </summary>
    public PartitionClause() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PartitionClause"/> class with the specified collection of values.
    /// </summary>
    /// <param name="collection">The collection of values.</param>
    public PartitionClause(List<ValueBase> collection) : base(collection)
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

        var clause = Token.Reserved(this, parent, "partition by");
        yield return clause;

        foreach (var item in base.GetTokens(clause)) yield return item;
    }

    public IEnumerable<ColumnValue> GetColumns()
    {
        foreach (var value in Items)
        {
            foreach (var item in value.GetColumns())
            {
                yield return item;
            }
        }
    }
}
