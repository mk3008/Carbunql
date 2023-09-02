using Carbunql.Clauses;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class Filter : IQueryCommand
{
	public WhereClause? WhereClause { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (WhereClause == null) yield break;

		var filterToken = Token.Reserved(this, parent, "filter");
		yield return filterToken;

		var bracket = Token.ReservedBracketStart(this, filterToken);
		yield return bracket;
		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetTokens(bracket)) yield return item;
		}
		yield return Token.ReservedBracketEnd(this, filterToken);
	}
}