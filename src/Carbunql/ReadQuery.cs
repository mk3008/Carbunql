using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

[Union(0, typeof(SelectQuery))]
[Union(1, typeof(ValuesQuery))]
public abstract class ReadQuery : IReadQuery
{
	public abstract SelectClause? GetSelectClause();

	public OperatableQuery? OperatableQuery { get; set; }

	public OrderClause? OrderClause { get; set; }

	public LimitClause? LimitClause { get; set; }

	public IReadQuery AddOperatableValue(string @operator, IReadQuery query)
	{
		if (OperatableQuery != null) throw new InvalidOperationException();
		OperatableQuery = new OperatableQuery(@operator, query);
		return query;
	}

	public abstract IEnumerable<SelectQuery> GetInternalQueries();

	public abstract IEnumerable<PhysicalTable> GetPhysicalTables();

	public IDictionary<string, object?> Parameters { get; set; } = new Dictionary<string, object?>();

	public virtual IDictionary<string, object?> GetInnerParameters() => EmptyParameters.Get();

	public IDictionary<string, object?> GetParameters()
	{
		var prm = EmptyParameters.Get();
		prm = prm.Merge(GetWithClause()?.GetParameters());
		prm = prm.Merge(GetSelectClause()?.GetParameters());
		prm = prm.Merge(GetInnerParameters());
		prm = prm.Merge(OperatableQuery?.GetParameters());
		prm = prm.Merge(OrderClause?.GetParameters());
		prm = prm.Merge(LimitClause?.GetParameters());
		prm = prm.Merge(Parameters);
		return prm;
	}

	public abstract IEnumerable<Token> GetCurrentTokens(Token? parent);

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		foreach (var item in GetCurrentTokens(parent)) yield return item;
		if (OperatableQuery != null) foreach (var item in OperatableQuery.GetTokens(parent)) yield return item;
		if (OrderClause != null) foreach (var item in OrderClause.GetTokens(parent)) yield return item;
		if (LimitClause != null) foreach (var item in LimitClause.GetTokens(parent)) yield return item;
	}

	public ReadQuery GetQuery()
	{
		return this;
	}

	public abstract WithClause? GetWithClause();

	public abstract SelectQuery GetOrNewSelectQuery();

	public abstract IEnumerable<string> GetColumnNames();

	public abstract SelectableTable ToSelectableTable(IEnumerable<string>? columnAliases);

	public string AddParameter(string name, object? Value)
	{
		Parameters.Add(name, Value);
		return name;
	}
}
