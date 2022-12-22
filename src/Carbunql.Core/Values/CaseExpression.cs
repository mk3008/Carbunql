using Carbunql.Core.Clauses;

namespace Carbunql.Core.Values;

public class CaseExpression : ValueBase
{
    public CaseExpression()
    {
    }

    public CaseExpression(ValueBase condition)
    {
        CaseCondition = condition;
    }

    public ValueBase? CaseCondition { get; init; }

    public List<WhenExpression> WhenExpressions { get; init; } = new();

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        var current = Token.Reserved(this, parent, "case");

        yield return current;
        if (CaseCondition != null) foreach (var item in CaseCondition.GetTokens(current)) yield return item;

        foreach (var item in WhenExpressions)
        {
            foreach (var token in item.GetTokens(current)) yield return token;
        }

        yield return Token.Reserved(this, parent, "end");
    }
}