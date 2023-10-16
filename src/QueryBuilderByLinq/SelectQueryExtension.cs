using Carbunql;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

internal static class SelectQueryExtension
{
	public static SelectableTable ToSelectable(this ParameterExpression prm)
	{
		var t = new PhysicalTable()
		{
			Table = prm.Type.ToTableName(),
			ColumnNames = prm.Type.GetProperties().ToList().Select(x => x.Name).ToList()
		};
		return t.ToSelectable();
	}

	public static SelectQuery AddJoinClause(this SelectQuery sq, LambdaExpression join, List<string> tables, ParameterExpression joinAlias)
	{
		var f = sq.FromClause!;

		var me = (MethodCallExpression)join.Body;

		if (me.Method.Name == "InnerJoin")
		{
			var arg = (UnaryExpression)me.Arguments.First();
			var lambda = (LambdaExpression)arg.Operand;

			var table = lambda.Parameters.First();
			var alias = table.Name!;
			var tablename = table.Type.ToTableName();

			var condition = lambda.ToValue(tables);

			// Replace the alias of the destination table name with the correct name.
			foreach (ColumnValue item in condition.GetValues().Where(x => x is ColumnValue c && c.TableAlias == alias))
			{
				item.TableAlias = joinAlias.Name!;
			}

			f.InnerJoin(joinAlias.ToSelectable()).As(joinAlias.Name!).On((_) => condition);
			return sq;
		}

		if (me.Method.Name == "LeftJoin")
		{
			var arg = (UnaryExpression)me.Arguments.First();
			var lambda = (LambdaExpression)arg.Operand;

			var table = lambda.Parameters.First();
			var alias = table.Name!;
			var tablename = table.Type.ToTableName();

			var condition = lambda.ToValue(tables);

			// Replace the alias of the destination table name with the correct name.
			foreach (ColumnValue item in condition.GetValues().Where(x => x is ColumnValue c && c.TableAlias == alias))
			{
				item.TableAlias = joinAlias.Name!;
			}

			f.LeftJoin(joinAlias.ToSelectable()).As(joinAlias.Name!).On((_) => condition);

			return sq;
		}

		throw new NotSupportedException();
	}

	public static SelectQuery AddSelectClause(this SelectQuery sq, LambdaExpression? select, LambdaExpression? where, List<string> tables)
	{
		if (select != null) return sq.AddSelectClauseBySelectExpression(select, tables);
		if (where != null) return sq.AddSelectClauseByWhereExpression(where, tables);
		return sq;
	}

	private static SelectQuery AddSelectClauseBySelectExpression(this SelectQuery sq, LambdaExpression select, List<string> tables)
	{
		var v = select.Body.ToValue(tables);

		if (v is ValueCollection vc)
		{
			sq.AddSelectClause(vc);
		}
		else
		{
			sq.Select(v).As(!string.IsNullOrEmpty(v.RecommendedName) ? v.RecommendedName : v.GetDefaultName());
		}

		var lst = sq.GetSelectableItems().Where(x => x.Value is ColumnValue c && c.TableAlias.StartsWith("<>h__TransparentIdentifier")).ToList();
		if (lst.Any())
		{
			foreach (var item in lst)
			{
				var t = sq.GetSelectableTables().Where(x => x.Alias == item.Alias).FirstOrDefault()?.Table;
				if (t != null)
				{
					foreach (var c in t.GetColumnNames())
					{
						sq.Select(item.Alias, c);
					}
				}
				sq.SelectClause!.Remove(item);

			}

		}
		return sq;
	}

	private static SelectQuery AddSelectClause(this SelectQuery sq, ValueCollection collection)
	{
		foreach (var item in collection)
		{
			if (item is ValueCollection vc)
			{
				sq.AddSelectClause(vc);
			}
			else
			{
				sq.Select(item).As(!string.IsNullOrEmpty(item.RecommendedName) ? item.RecommendedName : item.GetDefaultName());
			}
		}
		return sq;
	}

	private static SelectQuery AddSelectClauseByWhereExpression(this SelectQuery sq, LambdaExpression where, List<string> tables)
	{
		var tp = where.Parameters[0].Type;
		var alias = where.Parameters[0].Name!;
		tp.GetProperties().ToList().ForEach(x =>
		{
			sq.Select(alias, x.Name).As(x.Name);
		});
		return sq;
	}
}