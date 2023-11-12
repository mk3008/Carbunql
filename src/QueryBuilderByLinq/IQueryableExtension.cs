using Carbunql;
using Carbunql.Building;
using Carbunql.Clauses;
using QueryBuilderByLinq.Analysis;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

public static class IQueryableExtension
{
	public static SelectQuery ToSelectQuery(this IQueryable source)
	{
		var exp = (MethodCallExpression)source.Expression;

		var ctes = CommonTableInfoParser.Parse(exp);
		TableInfoParser.TryParse(exp, out var table);
		var joins = JoinTableInfoParser.Parse(exp);
		var aliases = GetAliases(table, joins);

		var wherevalue = WhereValueParser.Parse(exp, aliases);
		var selectitems = SelectableItemParser.Parse(exp, aliases);

		var sq = new SelectQuery();
		foreach (var item in ctes)
		{
			sq.With(item.Query.ToSelectQuery()).As(item.Alias);
		}
		if (table != null)
		{
			var (f, _) = sq.From(table.ToSelectable()).As(table.Alias);

			foreach (var join in joins)
			{
				var j = f.Join(join.TableInfo.ToSelectable(), join.Relation).As(join.TableInfo.Alias);
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

	private static List<string> GetAliases(TableInfo? table, List<JoinTableInfo> joins)
	{
		var aliases = new List<string>();
		if (table != null) aliases.Add(table.Alias);
		foreach (var item in joins)
		{
			aliases.Add(item.TableInfo.Alias);
		}

		return aliases;
	}
}