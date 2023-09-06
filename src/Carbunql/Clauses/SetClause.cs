using Carbunql.Tables;

namespace Carbunql.Clauses;

public class SetClause : QueryCommandCollection<ValueBase>, IQueryCommandable
{
	public IEnumerable<CommonTable> GetCommonTables()
	{
		foreach (var x in Items)
		{
			foreach (var item in x.GetCommonTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var x in Items)
		{
			foreach (var item in x.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		foreach (var x in Items)
		{
			foreach (var item in x.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		Token clause = GetClauseToken(parent);
		yield return clause;

		foreach (var item in base.GetTokens(clause)) yield return item;
	}

	private Token GetClauseToken(Token? parent)
	{
		return Token.Reserved(this, parent, "set");
	}
}