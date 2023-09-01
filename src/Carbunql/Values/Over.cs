using Carbunql.Clauses;

namespace Carbunql.Values;

[MessagePack.MessagePackObject]
public class Over : IQueryCommand
{
	[MessagePack.Key(0)]
	public PartitionClause? PartitionBy { get; set; }

	[MessagePack.Key(1)]
	public OrderClause? OrderBy { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (PartitionBy != null)
		{
			foreach (var item in PartitionBy.GetInternalQueries())
			{
				yield return item;
			}
		}
		if (OrderBy != null)
		{
			foreach (var item in OrderBy.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

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