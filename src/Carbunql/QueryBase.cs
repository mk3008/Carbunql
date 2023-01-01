﻿using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql;

public abstract class QueryBase : IQueryCommandable
{
    public WithClause? WithClause { get; set; }

    public OperatableQuery? OperatableQuery { get; private set; }

    public OrderClause? OrderClause { get; set; }

    public LimitClause? LimitClause { get; set; }

    public QueryBase AddOperatableValue(string @operator, QueryBase query)
    {
        if (OperatableQuery != null) throw new InvalidOperationException();
        OperatableQuery = new OperatableQuery(@operator, query);
        return query;
    }

    public IDictionary<string, object?>? Parameters { get; set; }

    public virtual IDictionary<string, object?> GetParameters()
    {
        var prm = EmptyParameters.Get();
        prm = prm.Merge(Parameters);
        prm = prm.Merge(OperatableQuery!.GetParameters());
        return prm;
    }

    public abstract IEnumerable<Token> GetCurrentTokens(Token? parent);

    public IEnumerable<Token> GetTokens(Token? parent = null)
    {
        if (WithClause != null) foreach (var item in WithClause.GetTokens(parent)) yield return item;
        foreach (var item in GetCurrentTokens(parent)) yield return item;
        if (OperatableQuery != null) foreach (var item in OperatableQuery.GetTokens(parent)) yield return item;
        if (OrderClause != null) foreach (var item in OrderClause.GetTokens(parent)) yield return item;
        if (LimitClause != null) foreach (var item in LimitClause.GetTokens(parent)) yield return item;
    }

    public String ToText()
    {
        return GetTokens(null).ToText();
    }

    public QueryCommand ToCommand()
    {
        return new QueryCommand(ToText(), GetParameters());
    }
}