using Carbunql.Values;

namespace Carbunql.Clauses;

public class ValuesQuery : ReadQuery
{
    public ValuesQuery(List<ValueCollection> rows)
    {
        Rows = rows;
    }

    public override SelectClause? GetSelectClause() => null;

    public List<ValueCollection> Rows { get; init; } = new();

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        var clause = Token.Reserved(this, parent, "values");
        yield return clause;

        var isFirst = true;
        foreach (var item in Rows)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                yield return Token.Comma(this, clause);
            }
            var bracket = Token.ReservedBracketStart(this, clause);
            yield return bracket;
            foreach (var token in item.GetTokens(bracket)) yield return token;
            yield return Token.ReservedBracketEnd(this, clause);
        }
    }
}