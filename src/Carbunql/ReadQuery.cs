using Carbunql.Clauses;
using Carbunql.Tables;
using MessagePack;

namespace Carbunql;

[Union(0, typeof(SelectQuery))]
[Union(1, typeof(ValuesQuery))]
public abstract class ReadQuery : IReadQuery
{
	public abstract SelectClause? GetSelectClause();

	public List<OperatableQuery> OperatableQueries { get; set; } = new();

	public OrderClause? OrderClause { get; set; }

	public LimitClause? LimitClause { get; set; }

	public IReadQuery AddOperatableValue(string @operator, IReadQuery query)
	{
		OperatableQueries.Add(new OperatableQuery(@operator, query));
		return query;
	}

	public abstract IEnumerable<SelectQuery> GetInternalQueries();

	public abstract IEnumerable<PhysicalTable> GetPhysicalTables();

	public abstract IEnumerable<CommonTable> GetCommonTables();

	public List<QueryParameter> Parameters { get; set; } = new();

	public virtual IEnumerable<QueryParameter> GetInnerParameters()
	{
		yield break;
	}

	public IEnumerable<QueryParameter> GetParameters()
	{
		var q = GetWithClause()?.GetParameters();
		if (q != null)
		{
			foreach (var item in q)
			{
				yield return item;
			}
		}
		q = GetSelectClause()?.GetParameters();
		if (q != null)
		{
			foreach (var item in q)
			{
				yield return item;
			}
		}
		q = GetInnerParameters();
		if (q != null)
		{
			foreach (var item in q)
			{
				yield return item;
			}
		}
		foreach (var oq in OperatableQueries)
		{
			foreach (var item in oq.GetParameters())
			{
				yield return item;
			}
		}
		q = OrderClause?.GetParameters();
		if (q != null)
		{
			foreach (var item in q)
			{
				yield return item;
			}
		}
		q = LimitClause?.GetParameters();
		if (q != null)
		{
			foreach (var item in q)
			{
				yield return item;
			}
		}
		foreach (var item in Parameters)
		{
			yield return item;
		}
	}

	public abstract IEnumerable<Token> GetCurrentTokens(Token? parent);

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		foreach (var item in GetCurrentTokens(parent)) yield return item;
		foreach (var oq in OperatableQueries)
		{
			foreach (var item in oq.GetTokens(parent)) yield return item;
		}
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
		Parameters.Add(new QueryParameter(name, Value));
		return name;
	}
}
