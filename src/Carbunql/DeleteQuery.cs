using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql;

public class DeleteQuery : IQueryCommandable
{
    public DeleteClause? DeleteClause { get; set; }

    public WhereClause? WhereClause { get; set; }

    public IDictionary<string, object?>? Parameters { get; set; }

    public virtual IDictionary<string, object?> GetParameters()
    {
        var prm = EmptyParameters.Get();
        prm = prm.Merge(Parameters);
        return prm;
    }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (DeleteClause == null) throw new NullReferenceException();

        foreach (var item in DeleteClause.GetTokens(parent)) yield return item;
        if (WhereClause != null) foreach (var item in WhereClause.GetTokens(parent)) yield return item;
    }
}