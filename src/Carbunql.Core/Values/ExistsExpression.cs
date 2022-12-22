namespace Carbunql.Core.Values;

public class ExistsExpression : QueryContainer
{
    public ExistsExpression(IQueryCommandable query) : base(query)
    {
    }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, "exists"); ;

        var bracket = Token.ReservedBracketStart(this, parent);
        yield return bracket;
        foreach (var item in Query.GetTokens(bracket)) yield return item;
        yield return Token.ReservedBracketEnd(this, parent);
    }
}
