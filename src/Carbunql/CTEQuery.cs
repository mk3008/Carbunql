using Carbunql.Clauses;

namespace Carbunql;

public class CTEQuery : QueryBase
{
    public CTEQuery()
    {
    }

    public CTEQuery(WithClause with)
    {
        WithClause = with;
    }

    public override WithClause WithClause { get; } = new();

    public QueryBase? Query { get; set; }

    public override QueryBase QueryWithoutCTE => GetQueryBase();

    private QueryBase GetQueryBase()
    {
        if (Query == null) throw new NullReferenceException(nameof(Query));
        return Query;
    }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        if (Query == null) throw new NullReferenceException(nameof(Query));
        foreach (var item in Query.GetTokens(parent)) yield return item;
    }
}