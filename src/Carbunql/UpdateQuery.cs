using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql;

public class UpdateQuery : IQueryCommandable
{
	public UpdateClause? UpdateClause { get; set; }

	public MergeSetClause? SetClause { get; set; }

	public FromClause? FromClause { get; set; }

	public WhereClause? WhereClause { get; set; }

	public IDictionary<string, object?>? Parameters { get; set; }

	public virtual IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(SetClause?.GetParameters());
		prm = prm.Merge(FromClause?.GetParameters());
		prm = prm.Merge(WhereClause?.GetParameters());
		prm = prm.Merge(Parameters);
		return prm;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (UpdateClause == null) throw new NullReferenceException();
		if (SetClause == null) throw new NullReferenceException();

		foreach (var item in UpdateClause.GetTokens(parent)) yield return item;
		foreach (var item in SetClause.GetTokens(parent)) yield return item;
		if (FromClause != null) foreach (var item in FromClause.GetTokens(parent)) yield return item;
		if (WhereClause != null) foreach (var item in WhereClause.GetTokens(parent)) yield return item;
	}
}