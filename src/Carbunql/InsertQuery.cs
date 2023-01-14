using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql;

public class InsertQuery : IQueryCommandable
{
    public InsertClause? InsertClause { get; set; }

    public IReadQuery? Query { get; set; }

    public IDictionary<string, object?>? Parameters { get; set; }

    public virtual IDictionary<string, object?> GetParameters()
    {
        var prm = EmptyParameters.Get();
        prm = prm.Merge(Parameters);
        return prm;
    }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (InsertClause == null) throw new NullReferenceException();

        foreach (var item in InsertClause.GetTokens(parent)) yield return item;
        if (Query != null) foreach (var item in Query.GetTokens(parent)) yield return item;
    }
}