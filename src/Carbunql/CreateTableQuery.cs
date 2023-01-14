using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql;

public class CreateTableQuery : IQueryCommandable
{
    public CreateTableQuery(CreateTableClause createTableClause)
    {
        CreateTableClause = createTableClause;
    }

    public CreateTableClause CreateTableClause { get; init; }

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
        foreach (var item in CreateTableClause.GetTokens(parent)) yield return item;
        var t = new Token(this, parent, "as", isReserved: true);
        yield return t;

        if (Query != null) foreach (var item in Query.GetTokens(t)) yield return item;
    }
}