using Carbunql.Clauses;

namespace Carbunql.Values;

//public class InExpression : QueryContainer
//{
//    public InExpression(IQueryCommandable query) : base(query)
//    {
//    }

//    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
//    {
//        yield return Token.Reserved(this, parent, "in");

//        var bracket = Token.ReservedBracketStart(this, parent);
//        yield return bracket;
//        foreach (var item in Query.GetTokens(bracket)) yield return item;
//        yield return Token.ReservedBracketEnd(this, parent);
//    }
//}

public class InExpression : ValueBase
{
    public InExpression(ValueBase value, ValueBase argument)
    {
        Value = value;
        Argument = argument;
    }

    public ValueBase Value { get; init; }

    public ValueBase Argument { get; init; }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;
        yield return Token.Reserved(this, parent, "in");
        foreach (var item in Argument.GetTokens(parent)) yield return item;
    }
}