using Carbunql.Clauses;

namespace Carbunql.Values;

public class QueryContainer : ValueBase
{
    public QueryContainer(IQueryCommandable query)
    {
        Query = query;
    }

    public IQueryCommandable Query { get; init; }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        var bracket = Token.ReservedBracketStart(this, parent);
        yield return bracket;
        foreach (var item in Query.GetTokens(bracket)) yield return item;
        yield return Token.ReservedBracketEnd(this, parent);
    }
}