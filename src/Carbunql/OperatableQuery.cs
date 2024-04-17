﻿using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

/// <summary>
/// query with operator
/// </summary>
[MessagePackObject(keyAsPropertyName: true)]
public class OperatableQuery : IQueryCommandable
{
    public OperatableQuery(string @operator, IReadQuery query)
    {
        Operator = @operator;
        Query = query;
    }

    /// <summary>
    /// "union", "union all", etc.
    /// </summary>
    public string Operator { get; init; }

    public IReadQuery Query { get; init; }

    public IEnumerable<QueryParameter> GetParameters()
    {
        return Query.GetParameters();
    }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var current = Token.Reserved(this, parent, Operator);
        yield return current;
        foreach (var item in Query.GetTokens(current)) yield return item;
    }

    public IEnumerable<SelectQuery> GetInternalQueries()
    {
        foreach (var item in Query.GetInternalQueries())
        {
            yield return item;
        }
    }

    public IEnumerable<PhysicalTable> GetPhysicalTables()
    {
        foreach (var item in Query.GetPhysicalTables())
        {
            yield return item;
        }
    }

    public IEnumerable<CommonTable> GetCommonTables()
    {
        foreach (var item in Query.GetCommonTables())
        {
            yield return item;
        }
    }
}