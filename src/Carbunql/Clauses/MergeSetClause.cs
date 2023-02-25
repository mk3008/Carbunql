namespace Carbunql.Clauses;

public class MergeSetClause : QueryCommandCollection<ValueBase>, IQueryCommandable
{
	public override IEnumerable<Token> GetTokens(Token? parent)
	{
		foreach (var item in base.GetTokens(parent)) yield return item;
	}
}