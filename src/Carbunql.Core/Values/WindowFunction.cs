namespace Carbunql.Core.Values;

public class WindowFunction : IQueryCommand
{
    public ValueCollection? PartitionBy { get; set; }

    public ValueCollection? OrderBy { get; set; }

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, "over");

        var bracket = Token.ReservedBracketStart(this, parent);
        yield return bracket;
        if (PartitionBy != null)
        {
            yield return Token.Reserved(this, parent, "partition by");
            foreach (var item in PartitionBy.GetTokens(bracket)) yield return item;
        }
        if (OrderBy != null)
        {
            yield return Token.Reserved(this, parent, "order by");
            foreach (var item in OrderBy.GetTokens(bracket)) yield return item;
        }
        yield return Token.ReservedBracketEnd(this, parent);
    }
}