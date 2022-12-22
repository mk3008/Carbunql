namespace Carbunql.Core.Clauses;

public class HavingClause : IQueryCommand
{
    public HavingClause(ValueBase condition)
    {
        Condition = condition;
    }

    public ValueBase Condition { get; init; }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "having");
        yield return clause;

        foreach (var item in Condition.GetTokens(clause)) yield return item;
    }
}