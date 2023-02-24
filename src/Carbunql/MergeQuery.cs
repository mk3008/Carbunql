using Carbunql.Clauses;
using Carbunql.Extensions;

namespace Carbunql;

public class MergeQuery : IQueryCommandable
{
	public WithClause? WithClause { get; set; }

	public MergeClause? MergeClause { get; set; }

	public UsingClause? UsingClause { get; set; }

	public WhenClause? WhenClause { get; set; }

	public IDictionary<string, object?>? Parameters { get; set; }

	public virtual IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(WithClause?.GetParameters());
		prm = prm.Merge(MergeClause?.GetParameters());
		prm = prm.Merge(UsingClause?.GetParameters());
		prm = prm.Merge(WhenClause?.GetParameters());
		prm = prm.Merge(Parameters);
		return prm;
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (MergeClause == null) throw new NullReferenceException();
		if (UsingClause == null) throw new NullReferenceException();
		if (WhenClause == null) throw new NullReferenceException();

		if (WithClause != null) foreach (var item in WithClause.GetTokens(parent)) yield return item;
		foreach (var item in MergeClause.GetTokens(parent)) yield return item;
		foreach (var item in UsingClause.GetTokens(parent)) yield return item;
		foreach (var item in WhenClause.GetTokens(parent)) yield return item;
	}
}