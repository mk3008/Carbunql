using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Values;

[MessagePackObject(keyAsPropertyName: true)]
public class Filter : IQueryCommandable
{
	public WhereClause? WhereClause { get; set; }

	public IEnumerable<CommonTable> GetCommonTables()
	{
		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetCommonTables())
			{
				yield return item;
			}
		}
	}

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

	public IEnumerable<QueryParameter> GetParameters()
	{
		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetParameters())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (WhereClause != null)
		{
			foreach (var item in WhereClause.GetPhysicalTables())
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