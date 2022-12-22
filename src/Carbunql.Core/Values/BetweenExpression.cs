using Carbunql.Core.Clauses;

namespace Carbunql.Core.Values;

public class BetweenExpression : ValueBase
{
    public BetweenExpression(ValueBase value, ValueBase start, ValueBase end)
    {
        Value = value;
        Start = start;
        End = end;
    }

    public ValueBase Value { get; init; }

    public ValueBase Start { get; init; }

    public ValueBase End { get; init; }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        foreach (var item in Value.GetCurrentTokens(parent)) yield return item;
        yield return Token.Reserved(this, parent, "between");
        foreach (var item in Start.GetCurrentTokens(parent)) yield return item;
        yield return Token.Reserved(this, parent, "and");
        foreach (var item in End.GetCurrentTokens(parent)) yield return item;
    }
}