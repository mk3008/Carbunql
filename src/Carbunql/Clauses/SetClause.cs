﻿using Carbunql.Tables;

namespace Carbunql.Clauses;

/// <summary>
/// Represents a SET clause in a query.
/// </summary>
public class SetClause : QueryCommandCollection<ValueBase>, IQueryCommandable
{
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
        return Token.Reserved(this, parent, "set");
    }
}
