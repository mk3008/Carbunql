namespace Carbunql.Clauses;

public class MergeWhenInsert : MergeCondition
{
    public MergeWhenInsert(MergeInsertQuery query)
    {
        Query = query;
    }

    public MergeInsertQuery Query { get; init; }

    public override IEnumerable<QueryParameter> GetParameters()
    {
        foreach (var item in Query.GetParameters())
        {
            yield return item;
        }
        if (Condition != null)
        {
            foreach (var item in Condition.GetParameters())
            {
                yield return item;
            }
        }
    }

    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        var t = Token.Reserved(this, parent, "when not matched");
        yield return t;
        foreach (var item in GetConditionTokens(t)) yield return item;
        yield return Token.Reserved(this, parent, "then");
        foreach (var item in Query.GetTokens(t)) yield return item;
    }
}