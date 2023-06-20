using Carbunql.Building;
using Carbunql.Extensions;
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

	public virtual SelectableTable ToSelectable(string alias, IEnumerable<string> columnAliases)
	{
		return new SelectableTable(this, alias, columnAliases.ToValueCollection());
	}

	public virtual SelectableTable ToSelectable(string alias, ValueCollection columnAliases)
	{
		return new SelectableTable(this, alias, columnAliases);
	}

	public virtual IDictionary<string, object?> GetParameters()
	{
		return EmptyParameters.Get();
	}

	public virtual IList<string> GetColumnNames()
	{
		return ImmutableList<string>.Empty;
	}

	public virtual bool IsSelectQuery => false;

	public virtual string GetTableFullName() => "";

	public virtual SelectQuery GetSelectQuery() => throw new NotSupportedException();
}