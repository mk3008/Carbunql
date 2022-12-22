namespace Carbunql.Core.Clauses;

public class WhereClause : IQueryCommand
{
    public WhereClause(ValueBase condition)
    {
        Condition = condition;
    }

    public ValueBase Condition { get; init; }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "where");
        yield return clause;
        foreach (var item in Condition.GetTokens(clause)) yield return item;
    }
}