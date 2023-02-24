using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql;

public class MergeUpdateQuery : IQueryCommandable
{
	public SetClause? SetClause { get; set; }

	public virtual IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(SetClause?.GetParameters());
		return prm;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (SetClause == null) throw new NullReferenceException();

		var t = Token.Reserved(this, parent, "update");
		yield return t;
		foreach (var item in SetClause.GetTokens(t)) yield return item;
	}
}