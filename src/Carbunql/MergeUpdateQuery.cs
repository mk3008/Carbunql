using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql;

public class MergeUpdateQuery : IQueryCommandable
{
	public MergeSetClause? SetClause { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (SetClause != null)
		{
			foreach (var item in SetClause.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public virtual IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(SetClause?.GetParameters());
		return prm;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (SetClause == null) throw new NullReferenceException();

		var t = Token.Reserved(this, parent, "update set");
		yield return t;
		foreach (var item in SetClause.GetTokens(t)) yield return item;
	}
}