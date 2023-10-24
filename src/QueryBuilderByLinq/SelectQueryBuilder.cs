using Carbunql;
using Carbunql.Building;
using Carbunql.Tables;
using Carbunql.Values;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

public class SelectQueryBuilder
{
	public SelectQueryBuilder(MethodCallExpression expression)
	{
		Expression = expression;
	}

	public MethodCallExpression Expression { get; init; }

	private LambdaExpression? GetSelectExpression(MethodCallExpression expression)
	{
		if (expression.Arguments.Count == 2)
		{
			var ue = (UnaryExpression)expression.Arguments[1];
			return GetSelectExpression(ue);
		}
		else if (expression.Arguments.Count == 3)
		{
			var ue = (UnaryExpression)expression.Arguments[2];
			return GetSelectExpression(ue);
		}
		throw new NotSupportedException();
	}

	private LambdaExpression? GetSelectExpression(UnaryExpression ue)
	{
		var operand = (LambdaExpression)ue.Operand;
		if (operand.ReturnType == typeof(bool)) return null;
		return operand;
	}

	private LambdaExpression? GetJoinExpression(MethodCallExpression expression)
	{
		if (expression.Arguments.Count == 3)
		{
			var ue = (UnaryExpression)expression.Arguments[1];
			return (LambdaExpression)ue.Operand;
		}
		return null;
	}

	private LambdaExpression? GetWhereExpression(MethodCallExpression expression)
	{
		if (expression.Arguments.Count == 2)
		{
			var ue = (UnaryExpression)expression.Arguments[1];
			var operand = (LambdaExpression)ue.Operand;
			if (operand.ReturnType != typeof(bool)) return null;
			return operand;
		}
		return null;
	}

	public SelectQuery Build(MethodCallExpression expression)
	{
		if (expression.Arguments[0] is ConstantExpression)
		{
			return BuildRootQuery(expression);
		}
		else if (expression.Arguments[0] is MethodCallExpression mce)
		{
			var sq = Build(mce);
			var x = BuildNestedQuery(expression, sq);
			return sq;
		}

		throw new NotSupportedException();
	}

	private SelectQuery BuildRootQuery(MethodCallExpression expression)
	{
		var root = (ConstantExpression)expression.Arguments[0];
		var nest = GetJoinExpression(expression);
		var select = GetSelectExpression(expression);
		var where = GetWhereExpression(expression);

		if (nest == null)
		{
			return BuildRootQuery(expression, root, select, where);
		}
		if (where == null && nest != null && nest.Body is MethodCallExpression mc && select != null)
		{
			if (mc.Method.Name == nameof(Sql.FromTable))
			{
				// CTE, From pattern
				return BuildCteQuery(expression, root, nest, select);
			}
			else if (mc.Method.Name == nameof(Sql.InnerJoinTable) || mc.Method.Name == nameof(Sql.LeftJoinTable) || mc.Method.Name == nameof(Sql.CrossJoinTable))
			{
				return BuildRootQuery(expression, root, nest, select, where);
			}
		}

		throw new NotSupportedException();
	}

	private SelectQuery BuildCteQuery(MethodCallExpression expression, ConstantExpression cte, LambdaExpression from, LambdaExpression select)
	{
		ParameterExpression? fromAlias = null;
		ParameterExpression? joinAlias = null;
		if (select != null)
		{
			fromAlias = select.Parameters.First();
			if (select.Parameters.Count > 1) joinAlias = select.Parameters.Last();
		}
		if (fromAlias == null) throw new NotSupportedException();

		var sq = new SelectQuery();
		if (fromAlias.Type != typeof(object))
		{
			if (cte.Value is IQueryable q && q.Provider is TableQuery tq)
			{
				if (tq.InnerQuery != null)
				{
					sq.With(tq.InnerQuery.ToQueryAsPostgres()).As(fromAlias.Name!);
				}
				else
				{
					throw new NotSupportedException();
				}
			}
		}
		return sq;
	}

	private SelectQuery BuildRootQuery(MethodCallExpression expression, ConstantExpression from, LambdaExpression join, LambdaExpression? select, LambdaExpression? where)
	{
		ParameterExpression? fromAlias = null;
		ParameterExpression? joinAlias = null;
		if (select != null)
		{
			fromAlias = select.Parameters.First();
			if (select.Parameters.Count > 1) joinAlias = select.Parameters.Last();
		}
		else if (where != null)
		{
			fromAlias = where.Parameters.First();
		}
		if (fromAlias == null) throw new NotSupportedException();

		var tables = new List<string> { fromAlias.Name! };

		var sq = new SelectQuery();
		if (fromAlias.Type != typeof(object))
		{
			if (from.Value is IQueryable q && q.Provider is TableQuery tq)
			{
				if (tq.InnerQuery != null)
				{
					sq.From(tq.InnerQuery.ToQueryAsPostgres()).As(fromAlias.Name!);
				}
				else
				{
					sq.From(tq.TableName).As(fromAlias.Name!);
				}
			}
			else
			{
				sq.From(fromAlias.ToSelectable()).As(fromAlias.Name!);
			}

			if (where != null) sq.Where(where.ToValue(tables));
			if (join != null && joinAlias != null)
			{
				sq.AddJoinClause(join, tables, joinAlias);
			}
		}
		return sq.AddSelectClause(select, where, tables);
	}

	private SelectQuery BuildRootQuery(MethodCallExpression expression, ConstantExpression from, LambdaExpression? select, LambdaExpression? where)
	{
		ParameterExpression? fromAlias = null;
		ParameterExpression? joinAlias = null;
		if (select != null)
		{
			fromAlias = select.Parameters.First();
			if (select.Parameters.Count > 1) joinAlias = select.Parameters.Last();
		}
		else if (where != null)
		{
			fromAlias = where.Parameters.First();
		}
		if (fromAlias == null) throw new NotSupportedException();

		var tables = new List<string> { fromAlias.Name! };

		var sq = new SelectQuery();
		if (fromAlias.Type != typeof(object))
		{
			if (from.Value is IQueryable q && q.Provider is TableQuery tq)
			{
				if (tq.InnerQuery != null)
				{
					sq.From(tq.InnerQuery.ToQueryAsPostgres()).As(fromAlias.Name!);
				}
				else
				{
					sq.From(tq.TableName).As(fromAlias.Name!);
				}
			}
			else
			{
				sq.From(fromAlias.ToSelectable()).As(fromAlias.Name!);
			}

			if (where != null) sq.Where(where.ToValue(tables));
		}
		return sq.AddSelectClause(select, where, tables);
	}

	private ParameterExpression? GetJoinAlias(LambdaExpression? select, LambdaExpression? where)
	{
		if (select != null && select.Parameters.Count > 1)
		{
			return select.Parameters.Last();
		}
		else if (where != null)
		{
			return where.Parameters.First();
		}
		return null;
	}

	private SelectQuery BuildNestedQuery(MethodCallExpression expression, SelectQuery sq)
	{
		var where = GetWhereExpression(expression);
		var join = GetJoinExpression(expression);
		var select = GetSelectExpression(expression);
		var joinAlias = GetJoinAlias(select, where);

		if (sq.FromClause == null)
		{
			if (expression.Arguments.Count == 2)
			{
				var exp = (MethodCallExpression)expression.Arguments[0];
				sq = BuildRootQuery(exp, sq);

				var ts = sq.GetSelectableTables().Select(x => x.Alias).ToList();
				if (where != null) sq.Where(where.ToValue(ts));
				return sq;
			}
			if (expression.Arguments.Count == 3 && join != null && joinAlias != null)
			{
				var exp = (MethodCallExpression)expression.Arguments[0];
				sq = BuildRootQuery(exp, sq);

				var ts = sq.GetSelectableTables().Select(x => x.Alias).ToList();
				ts.Add(joinAlias.Name!);
				sq.AddJoinClause(join, ts, joinAlias);

				if (where != null) sq.Where(where.ToValue(ts));

				return sq;
			}
		}

		if (sq.FromClause == null) throw new NotSupportedException();
		{
			var tables = sq.GetSelectableTables().Select(x => x.Alias).ToList();

			if (join != null && joinAlias != null)
			{
				tables.Add(joinAlias.Name!);
				sq.AddJoinClause(join, tables, joinAlias);
			}

			if (where != null) sq.Where(where.ToValue(tables));

			//refresh select clause
			if (select != null)
			{
				sq.SelectClause = null;
				sq.AddSelectClause(select, where, tables);
			}
			return sq;
		}
	}

	private SelectQuery BuildRootQuery(MethodCallExpression expression, SelectQuery sq)
	{
		var select = GetSelectExpression(expression);

		if (select == null || select.Parameters.Count != 2) throw new NotSupportedException();

		var table = select.Parameters[0];
		var alias = select.Parameters[1];

		if (string.IsNullOrEmpty(table?.Name)) throw new NotSupportedException();
		if (string.IsNullOrEmpty(alias?.Name)) throw new NotSupportedException();

		var tables = new List<string> { alias.Name! };
		//if (where != null) sq.Where(where.ToValue(tables));

		var v = (ValueCollection)select.Body.ToValue(tables);

		var columns = (ValueCollection)v.First();
		var columnnames = columns.Select(x => ((ColumnValue)x).Column).ToList();

		foreach (var column in columnnames)
		{
			sq.Select(alias!.Name, column);
		}

		var pt = new PhysicalTable()
		{
			ColumnNames = columnnames,
			Table = table.Name
		};

		sq.From(pt.ToSelectable()).As(alias.Name);

		return sq;
	}
}