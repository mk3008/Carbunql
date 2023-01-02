using Carbunql.Clauses;

namespace Carbunql.Values;

public class WindowFunction : IQueryCommand
{
    public PartitionClause? PartitionBy { get; set; }

    public OrderClause? OrderBy { get; set; }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        if (PartitionBy == null && OrderBy == null) yield break;

        var overToken = Token.Reserved(this, parent, "over");
        yield return overToken;

        var bracket = Token.ReservedBracketStart(this, overToken);
        yield return bracket;
        if (PartitionBy != null)
        {
            foreach (var item in PartitionBy.GetTokens(bracket)) yield return item;
        }
        if (OrderBy != null)
        {
            foreach (var item in OrderBy.GetTokens(bracket)) yield return item;
        }
        yield return Token.ReservedBracketEnd(this, overToken);
    }
}