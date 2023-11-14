using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Postgres;
using System.Linq.Expressions;

namespace Carbunql.Postgres.Linq;

public static class IQueryableExtension
{
	public static SelectQuery ToSelectQuery(this IQueryable source)
	{
		var exp = (MethodCallExpression)source.Expression;

		var ctes = CommonTableInfoParser.Parse(exp);
		SelectableTableParser.TryParse(exp, out var table);
		var joins = JoinTableInfoParser.Parse(exp);
		var aliases = GetAliases(table, joins);

		var wherevalue = WhereValueParser.Parse(exp, aliases.Select(x => x.Alias).ToList());
		var selectitems = SelectableItemParser.Parse(exp, aliases);

		var sq = new SelectQuery();
		foreach (var item in ctes)
		{
			sq.With(item.Query.ToSelectQuery()).As(item.Alias);
		}
		if (table != null)
		{
			var (f, _) = sq.From(table).As(table.Alias);

			foreach (var join in joins)
			{
				var j = f.Join(join.Table, join.Relation).As(join.Table.Alias);
				if (join.Condition != null) j.On(_ => join.Condition);
			}
		}
		foreach (var item in selectitems)
		{
			sq.SelectClause ??= new();
			sq.SelectClause.Add(item);
		}
		if (wherevalue != null)
		{
			sq.Where(wherevalue);
		}
		return sq;
	}

	private static List<SelectableTable> GetAliases(SelectableTable? table, List<JoinTableInfo> joins)
	{
		var aliases = new List<SelectableTable>();
		if (table != null) aliases.Add(table);
		foreach (var item in joins)
		{
			aliases.Add(item.Table);
		}

		return aliases;
	}
}