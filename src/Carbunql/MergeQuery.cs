using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Tables;
using Carbunql.Values;

namespace Carbunql;

public class MergeQuery : IQueryCommandable
{
	public MergeQuery(IReadQuery datasource, string destinationTable, IEnumerable<string> keys, string datasourceAlias = "s", string destinationAlias = "d")
	{
		var destination = CreateDestination(datasource, destinationTable, keys, destinationAlias);

		Datasource = datasource;
		DatasourceAlias = datasourceAlias;

		WithClause = datasource.GetWithClause();
		UsingClause = CreateUsingClause(datasource, keys, destination.Alias, datasourceAlias);
		MergeClause = new MergeClause(destination);
		Parameters = datasource.GetParameters();
	}

	public MergeQuery(IReadQuery datasource, SelectableTable destination, IEnumerable<string> keys, string datasourceAlias = "s")
	{
		Datasource = datasource;
		DatasourceAlias = datasourceAlias;

		WithClause = datasource.GetWithClause();
		UsingClause = CreateUsingClause(datasource, keys, destination.Alias, datasourceAlias);
		MergeClause = new MergeClause(destination);
		Parameters = datasource.GetParameters();
	}

	public WithClause? WithClause { get; set; }

	public MergeClause MergeClause { get; set; }

	public UsingClause UsingClause { get; set; }

	public WhenClause? WhenClause { get; set; }

	public IEnumerable<QueryParameter>? Parameters { get; set; }

	public IReadQuery Datasource { get; init; }

	public string DatasourceAlias { get; set; }

	public IEnumerable<SelectQuery> GetInternalQueries()
	{
		if (WithClause != null)
		{
			foreach (var item in WithClause.GetInternalQueries())
			{
				yield return item;
			}
		}
		foreach (var item in MergeClause.GetInternalQueries())
		{
			yield return item;
		}
		foreach (var item in UsingClause.GetInternalQueries())
		{
			yield return item;
		}
		if (WhenClause != null)
		{
			foreach (var item in WhenClause.GetInternalQueries())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<PhysicalTable> GetPhysicalTables()
	{
		if (WithClause != null)
		{
			foreach (var item in WithClause.GetPhysicalTables())
			{
				yield return item;
			}
		}
		foreach (var item in MergeClause.GetPhysicalTables())
		{
			yield return item;
		}
		foreach (var item in UsingClause.GetPhysicalTables())
		{
			yield return item;
		}
		if (WhenClause != null)
		{
			foreach (var item in WhenClause.GetPhysicalTables())
			{
				yield return item;
			}
		}
	}

	public IEnumerable<CommonTable> GetCommonTables()
	{
		if (WithClause != null)
		{
			foreach (var item in WithClause.GetCommonTables())
			{
				yield return item;
			}
		}
		foreach (var item in MergeClause.GetCommonTables())
		{
			yield return item;
		}
		foreach (var item in UsingClause.GetCommonTables())
		{
			yield return item;
		}
		if (WhenClause != null)
		{
			foreach (var item in WhenClause.GetCommonTables())
			{
				yield return item;
			}
		}
	}

	public virtual IEnumerable<QueryParameter> GetParameters()
	{
		if (WithClause != null)
		{
			foreach (var item in WithClause.GetParameters())
			{
				yield return item;
			}
		}
		if (MergeClause != null)
		{
			foreach (var item in MergeClause.GetParameters())
			{
				yield return item;
			}
		}
		if (UsingClause != null)
		{
			foreach (var item in UsingClause.GetParameters())
			{
				yield return item;
			}
		}
		if (WhenClause != null)
		{
			foreach (var item in WhenClause.GetParameters())
			{
				yield return item;
			}
		}
		if (Parameters != null)
		{
			foreach (var item in Parameters)
			{
				yield return item;
			}
		}
	}

	public IEnumerable<Token> GetTokens(Token? parent)
	{
		if (WhenClause == null) throw new NullReferenceException();

		if (WithClause != null) foreach (var item in WithClause.GetTokens(parent)) yield return item;
		foreach (var item in MergeClause.GetTokens(parent)) yield return item;
		foreach (var item in UsingClause.GetTokens(parent)) yield return item;
		foreach (var item in WhenClause.GetTokens(parent)) yield return item;
	}

	private SelectableTable CreateDestination(IReadQuery datasource, string destinationTable, IEnumerable<string> keys, string destinationAlias)
	{
		var s = datasource.GetSelectClause();
		if (s == null) throw new NotSupportedException("select clause is not found.");
		return new SelectableTable(new PhysicalTable(destinationTable), destinationAlias);
	}

	private UsingClause CreateUsingClause(IReadQuery datasource, IEnumerable<string> keys, string destinationName, string sourceName)
	{
		var s = datasource.GetSelectClause();
		if (s == null) throw new NotSupportedException("select clause is not found.");
		var cols = s.Where(x => keys.Contains(x.Alias)).Select(x => x.Alias).ToList();

		ValueBase? v = null;
		foreach (var item in cols)
		{
			if (v == null)
			{
				v = new ColumnValue(destinationName, item);
			}
			else
			{
				v.And(new ColumnValue(destinationName, item));
			}
			v.Equal(new ColumnValue(sourceName, item));
		};
		if (v == null) throw new Exception();

		return new UsingClause(datasource.ToSelectableTable(sourceName), v, keys);
	}
}