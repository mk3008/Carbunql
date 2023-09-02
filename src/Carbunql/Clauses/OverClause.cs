using MessagePack;

namespace Carbunql.Clauses;

[MessagePackObject(keyAsPropertyName: true)]
public class OverClause : IQueryCommand
{
    public OverClause()
    {
        WindowDefinition = null!;
	}

	public OverClause(WindowDefinition definition)
	{
		WindowDefinition = definition;
	}

	public WindowDefinition WindowDefinition { get; set; }

    public IEnumerable<SelectQuery> GetInternalQueries()
    {
		foreach (var item in WindowDefinition.GetInternalQueries())
		{
			yield return item;
		}
	}

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        var overToken = Token.Reserved(this, parent, "over");
        yield return overToken;

        var bracket = Token.ReservedBracketStart(this, overToken);
        yield return bracket;
		
        foreach (var item in WindowDefinition.GetTokens(bracket)) yield return item;
		
        yield return Token.ReservedBracketEnd(this, overToken);
    }
}