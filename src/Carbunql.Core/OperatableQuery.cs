namespace Carbunql.Core;

public class OperatableQuery : IQueryCommandable
{
    public OperatableQuery(string @operator, QueryBase query)
    {
        Operator = @operator;
        Query = query;
    }

    public string Operator { get; init; }

    public QueryBase Query { get; init; }

    public IDictionary<string, object?> GetParameters()
    {
        return Query.GetParameters();
    }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var current = Token.Reserved(this, parent, Operator);
        yield return current;
        foreach (var item in Query.GetTokens(current)) yield return item;
    }

    public QueryCommand ToCommand()
    {
        return Query.ToCommand();
    }
}