namespace Carbunql.Clauses;

public class MergeSetClause : QueryCommandCollection<ValueBase>, IQueryCommandable
{
	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		foreach (var value in Items)
		{
			foreach (var item in value.GetInternalQueries())
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