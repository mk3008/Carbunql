using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a USING statement in SQL.
/// </summary>
public class DeleteUsingClause : QueryCommandCollection<SelectableTable>, IQueryCommandable
{
    public IEnumerable<ColumnValue> GetColumns()
    {
        foreach (var x in Items)
        {
            foreach (var item in x.GetColumns())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var x in Items)
        {
            foreach (var item in x.GetCommonTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var x in Items)
        {
            foreach (var item in x.GetInternalQueries())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var x in Items)
        {
            foreach (var item in x.GetPhysicalTables())
            {
                yield return item;
            }
        }
    }

    /// <inheritdoc/>
    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        Token clause = GetClauseToken(parent);
        yield return clause;

        foreach (var item in base.GetTokens(clause)) yield return item;
    }

    private Token GetClauseToken(Token? parent)
    {
        return Token.Reserved(this, parent, "using");
    }
}
