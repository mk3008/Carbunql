using Carbunql.Clauses;
using Carbunql.Values;
using Carbunql.Extensions;

namespace Carbunql.Building;

public static class MergeQueryExtensions
{
	private static MergeWhenInsert ToMergeWhenInsert(this MergeQuery source, IEnumerable<string> ignoreColumns)
	{
		var s = source.Datasource.GetSelectClause();
		if (s == null) throw new NotSupportedException("select clause is not found.");

		var vals = s.Select(x => x.Alias).Where(x => !x.IsEqualNoCase(ignoreColumns)).ToList();
		var q = new MergeInsertQuery()
		{
			Destination = vals.ToValueCollection(),
			Datasource = vals.ToValueCollection(source.DatasourceAlias)
		};
		return new MergeWhenInsert(q);
	}

	public static void AddNotMathcedInsertAsAutoNumber(this MergeQuery source)
	{
		var m = source.ToMergeWhenInsert(source.UsingClause.Keys);

		//key1 is null and key2 is null and ... keyN is null
		ValueBase? v = null;
		foreach (var item in source.UsingClause.Keys)
		{
			if (v == null)
			{
				v = new ColumnValue(source.DatasourceAlias, item);
			}
			else
			{
				v.And(new ColumnValue(source.DatasourceAlias, item));
			}
			v.IsNull();
		};
		if (v == null) throw new Exception();

		m.Condition = v;

		source.WhenClause ??= new();
		source.WhenClause.Add(m);
	}

	public static void AddNotMatchedInsert(this MergeQuery source)
	{
		var m = source.ToMergeWhenInsert(Enumerable.Empty<string>());

		source.WhenClause ??= new();
		source.WhenClause.Add(m);
	}

	private static MergeWhenUpdate ToMergeWhenUpdate(this MergeQuery source)
	{
		var s = source.Datasource.GetSelectClause();
		if (s == null) throw new NotSupportedException("select clause is not found.");
		var cols = s.Where(x => !source.UsingClause.Keys.Contains(x.Alias)).Select(x => x.Alias).ToList();

		var clause = new MergeSetClause();
		foreach (var item in cols)
		{
			var c = new ColumnValue(item);
			c.Equal(new ColumnValue(source.DatasourceAlias, item));
			clause.Add(c);
		};

		var q = new MergeUpdateQuery() { SetClause = clause };
		return new MergeWhenUpdate(q);
	}

	public static void AddMatchedUpdate(this MergeQuery source)
	{
		var m = source.ToMergeWhenUpdate();

		source.WhenClause ??= new();
		source.WhenClause.Add(m);
	}
}
