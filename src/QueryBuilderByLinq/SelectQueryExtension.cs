using Carbunql;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Tables;
using Carbunql.Values;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

internal static class Queryable
{
	public static bool TryParse(ConstantExpression @const, out IQueryable query)
	{
		query = null!;

		if (@const.Value is IQueryable q && q.Provider is TableQuery tq)
		{
			if (tq.InnerQuery != null)
			{
				query = tq.InnerQuery;
				return true;
			}
		}
		return false;
	}

	public static bool TryParse(MethodCallExpression method, out IQueryable query)
	{
		query = null!;

		if (!method.Arguments.Any()) return false;

		if (!(method.Method.Name == nameof(Sql.InnerJoinTable) || method.Method.Name == nameof(Sql.LeftJoinTable) || method.Method.Name == nameof(Sql.CrossJoinTable) || method.Method.Name == nameof(Sql.CommonTable)))
		{
			return false;
		}

		if (method.Arguments[0] is MemberExpression mem && mem.Expression is ConstantExpression ce)
		{
			var fieldname = mem.Member.Name;
			var val = ce.Value;
			if (val == null) return false;
			var tp = val.GetType();
			if (!tp.GetFields().Any()) return false;
			var field = tp.GetFields().Where(x => x.Name == fieldname).FirstOrDefault();
			if (field == null) return false;
			if (field.GetValue(ce.Value) is IQueryable q)
			{
				query = q;
				return true;
			}
		}

		return false;
	}
}



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

	//public static bool GetJoinQuery(this MethodCallExpression exp, out IQueryable? query)
	//{
	//	query = null;
	//	if (!exp.Arguments.Any()) return false;

	//	if (!(exp.Method.Name == nameof(Sql.InnerJoinTable) || exp.Method.Name == nameof(Sql.LeftJoinTable) || exp.Method.Name == nameof(Sql.CrossJoinTable) || exp.Method.Name == nameof(Sql.CommonTable2)))
	//	{
	//		return false;
	//	}

	//	if (exp.Arguments[0] is MemberExpression mem && mem.Expression is ConstantExpression ce)
	//	{
	//		var fieldname = mem.Member.Name;
	//		var val = ce.Value;
	//		if (val == null) return false;
	//		var tp = val.GetType();
	//		if (!tp.GetFields().Any()) return false;
	//		var field = tp.GetFields().Where(x => x.Name == fieldname).FirstOrDefault();
	//		if (field == null) return false;
	//		if (field.GetValue(ce.Value) is IQueryable q)
	//		{
	//			query = q;
	//			return true;
	//		}
	//		return false;
	//	}

	//	return false;
	//}

	private static bool GetJoinTableName(this MethodCallExpression exp, out string tablename)
	{
		tablename = string.Empty;
		if (!exp.Arguments.Any()) return false;

		if (!(exp.Method.Name == nameof(Sql.InnerJoinTable) || exp.Method.Name == nameof(Sql.LeftJoinTable) || exp.Method.Name == nameof(Sql.CrossJoinTable)))
		{
			return false;
		}

		var name = exp.GetArgument<MemberExpression>(index: 0)?.Member?.Name;

		if (name != null)
		{
			tablename = name;
			return true;
		}
		return false;
	}

	public static SelectQuery AddJoinClause(this SelectQuery sq, LambdaExpression join, List<string> tables, ParameterExpression joinAlias)
	{
		var me = (MethodCallExpression)join.Body;

		if (me.Method.Name == nameof(Sql.FromTable) && sq.FromClause == null)
		{
			var arg = (ConstantExpression)me.Arguments[0];
			var table = arg.Value?.ToString();
			var alias = joinAlias.Name;
			if (string.IsNullOrEmpty(table)) throw new InvalidProgramException();
			if (string.IsNullOrEmpty(alias)) throw new InvalidProgramException();

			var t = new PhysicalTable()
			{
				Table = table,
				//ColumnNames = prm.Type.GetProperties().ToList().Select(x => x.Name).ToList()
			};
			sq.From(t.ToSelectable()).As(alias);
			return sq;
		}

		if (sq.FromClause == null)
		{
			throw new InvalidProgramException();
		}
		var f = sq.FromClause!;

		if (me.Method.Name == nameof(Sql.InnerJoinTable))
		{
			var arg = (UnaryExpression)me.Arguments.Where(x => x is UnaryExpression).First();
			var lambda = (LambdaExpression)arg.Operand;

			var alias = lambda.Parameters.First().Name!;
			var condition = lambda.ToValue(tables);

			// Replace the alias of the destination table name with the correct name.
			foreach (ColumnValue item in condition.GetValues().Where(x => x is ColumnValue c && c.TableAlias == alias))
			{
				item.TableAlias = joinAlias.Name!;
			}

			if (Queryable.TryParse(me, out var subq))
			{
				f.InnerJoin(subq.ToQueryAsPostgres()).As(joinAlias.Name!).On((_) => condition);
			}
			else if (me.GetJoinTableName(out var name))
			{
				var cte = sq.WithClause?.Where(x => x.Alias == name).FirstOrDefault();
				if (cte != null && !string.IsNullOrEmpty(cte.Alias))
				{
					var t = new PhysicalTable()
					{
						Table = cte.Alias,
						ColumnNames = cte.GetColumnNames().ToList()
					};
					f.InnerJoin(t.ToSelectable()).As(joinAlias.Name!).On((_) => condition);
				}
				else
				{
					f.InnerJoin(name).As(joinAlias.Name!).On((_) => condition);
				}
			}
			else
			{
				f.InnerJoin(joinAlias.ToSelectable()).As(joinAlias.Name!).On((_) => condition);
			}
			return sq;
		}

		if (me.Method.Name == nameof(Sql.LeftJoinTable))
		{
			var arg = (UnaryExpression)me.Arguments.Where(x => x is UnaryExpression).First();
			var lambda = (LambdaExpression)arg.Operand;

			var alias = lambda.Parameters.First().Name!;
			var condition = lambda.ToValue(tables);

			// Replace the alias of the destination table name with the correct name.
			foreach (ColumnValue item in condition.GetValues().Where(x => x is ColumnValue c && c.TableAlias == alias))
			{
				item.TableAlias = joinAlias.Name!;
			}

			if (Queryable.TryParse(me, out var subq))
			{
				f.LeftJoin(subq.ToQueryAsPostgres()).As(joinAlias.Name!).On((_) => condition);
			}
			else if (me.GetJoinTableName(out var name))
			{
				var cte = sq.WithClause?.Where(x => x.Alias == name).FirstOrDefault();
				if (cte != null && !string.IsNullOrEmpty(cte.Alias))
				{
					var t = new PhysicalTable()
					{
						Table = cte.Alias,
						ColumnNames = cte.GetColumnNames().ToList()
					};
					f.LeftJoin(t.ToSelectable()).As(joinAlias.Name!).On((_) => condition);
				}
				else
				{
					f.LeftJoin(name).As(joinAlias.Name!).On((_) => condition);
				}
			}
			else
			{
				f.LeftJoin(joinAlias.ToSelectable()).As(joinAlias.Name!).On((_) => condition);
			}
			return sq;
		}

		if (me.Method.Name == nameof(Sql.CrossJoinTable))
		{
			if (Queryable.TryParse(me, out var subq))
			{
				f.CrossJoin(subq.ToQueryAsPostgres()).As(joinAlias.Name!);
			}
			else if (me.GetJoinTableName(out var name))
			{
				f.CrossJoin(name).As(joinAlias.Name!);
			}
			else
			{
				f.CrossJoin(joinAlias.ToSelectable()).As(joinAlias.Name!);
			}
			return sq;
		}

		throw new NotSupportedException($"Method name:{me.Method.Name}");
	}

	public static SelectQuery AddSelectClause(this SelectQuery sq, LambdaExpression? select, LambdaExpression? where, List<string> tables)
	{
		if (select != null)
		{
			return sq.AddSelectClauseBySelectExpression(select, tables);
		}
		if (where != null)
		{
			return sq.AddSelectClauseByWhereExpression(where, tables);
		}
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

		//rename table alias for SelectAll syntax.
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

	internal static SelectQuery AddSelectClause(this SelectQuery sq, ValueCollection collection)
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