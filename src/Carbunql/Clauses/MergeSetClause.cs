namespace Carbunql.Clauses;

public class MergeSetClause : QueryCommandCollection<ValueBase>, IQueryCommandable
{
	public IEnumerable<SelectQuery> GetSelectQueries()
	{
		foreach (var value in Items)
		{
			foreach (var item in value.GetSelectQueries())
			{
				yield return item;
			}
		}
	}

	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		foreach (var item in base.GetTokens(parent)) yield return item;
	}
}