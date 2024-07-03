using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a clause for the "SET" part of a "MERGE" SQL statement.
/// </summary>
public class MergeSetClause : QueryCommandCollection<ValueBase>, IQueryCommandable
{
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
    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var item in base.GetTokens(parent)) yield return item;
    }
}
