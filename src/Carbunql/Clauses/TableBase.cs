using Carbunql.Values;
using System.Collections.Immutable;

namespace Carbunql.Clauses;

public abstract class TableBase : IQueryCommandable
{
	public abstract IEnumerable<Token> GetTokens(Token? parent);

	public virtual string GetDefaultName() => string.Empty;

	public virtual SelectableTable ToSelectable() => ToSelectable(GetDefaultName());

	public virtual SelectableTable ToSelectable(string alias)
	{
		return new SelectableTable(this, alias);
	}

	public virtual SelectableTable ToSelectable(string alias, ValueCollection columnAliases)
	{
		return new SelectableTable(this, alias, columnAliases);
	}

	public virtual IDictionary<string, object?> GetParameters()
	{
		return EmptyParameters.Get();
	}

	public virtual IList<string> GetValueNames()
	{
		return ImmutableList<string>.Empty;
	}
}