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

	public SelectQuery Build(MethodCallExpression expression)
	{
		if (expression.Arguments[0] is ConstantExpression)
		{
			return BuildRootQuery(expression);
		}
		else if (expression.Arguments[0] is MethodCallExpression mce)
		{
			var sq = Build(mce);
			sq = BuildNestedQuery(expression, sq);
			return sq;
		}

		throw new NotSupportedException();
	}

	private SelectQuery BuildRootQuery(MethodCallExpression expression)
	{
		var root = expression.GetArgument<ConstantExpression>(index: 0);
		var method = expression.GetMethodLambdaFromArguments();
		var select = expression.GetSelectLambdaFromArguments();
		var condition = expression.GetConditionLambdaFromArguments();

		if (root != null && method == null)
		{
			// no relation pattern
			return BuildRootQuery(expression, root, select, condition);
		}
		if (condition == null && method != null && method.Body is MethodCallExpression nestBody && select != null)
		{
			if (nestBody.Method.Name == nameof(Sql.FromTable))
			{
				// CTE, From pattern
				return BuildCteQuery(expression, root, method, select);
			}
			else if (nestBody.Method.Name == nameof(Sql.CommonTable))
			{
				// CTE, CTE pattern
				return BuildCteQuery(expression, root, method, select);
			}
			else if (nestBody.Method.Name == nameof(Sql.InnerJoinTable) || nestBody.Method.Name == nameof(Sql.LeftJoinTable) || nestBody.Method.Name == nameof(Sql.CrossJoinTable))
			{
				// from, relation pattern
				return BuildRootQuery(expression, root, method, select, condition);
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
			var prm = expression.Method.GetParameters();
			fromAlias = select.Parameters.First();
			if (select.Parameters.Count > 1) joinAlias = select.Parameters.Last();
		}
		if (fromAlias == null) throw new NotSupportedException();



		var sq = new SelectQuery();
		if (fromAlias.Type != typeof(object))
		{
			if (Queryable.TryParse(cte, out var cte1))
			{
				sq.With(cte1.ToQueryAsPostgres()).As(fromAlias.Name!);
			}
			else
			{
				throw new NotSupportedException();
			}

			if (from.Body is MethodCallExpression fromBody && Queryable.TryParse(fromBody, out var cte2))
			{
				sq.With(cte2.ToQueryAsPostgres()).As(joinAlias!.Name!);
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

	private SelectQuery BuildRootQuery(MethodCallExpression expression, SelectQuery sq, LambdaExpression? where)
	{
		var exp = (MethodCallExpression)expression.Arguments[0];
		sq = BuildRootQuery(exp, sq);

		var ts = sq.GetSelectableTables().Select(x => x.Alias).ToList();
		if (where != null) sq.Where(where.ToValue(ts));
		return sq;
	}




	private LambdaExpression? GetArgumentOperand(MethodCallExpression expression, int argumentIndex)
	{
		if (expression.Arguments.Count() < argumentIndex + 1) return null;
		if (expression.Arguments[argumentIndex] is UnaryExpression u && u.Operand is LambdaExpression lam)
		{
			return lam;
		}
		return null;
	}

	private T? GetArgumentOperandBody<T>(MethodCallExpression expression, int argumentIndex)
	{
		var lam = GetArgumentOperand(expression, argumentIndex);
		if (lam == null) return default(T);
		if (lam.Body is T body) return body;
		return default(T);
	}

	private SelectQuery BuildNestedQuery(MethodCallExpression expression, SelectQuery sq)
	{
		var condition = expression.GetConditionLambdaFromArguments();
		var method = expression.GetMethodLambdaFromArguments();
		var select = expression.GetSelectLambdaFromArguments();
		var joinAlias = GetJoinAlias(select, condition);

		if (sq.FromClause == null)
		{
			if (expression.Arguments.Count == 2)
			{
				return BuildRootQuery(expression, sq, condition);
			}
			if (expression.Arguments.Count == 3 && method != null && joinAlias != null)
			{
				var mc = expression.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>().GetBody<MethodCallExpression>(); ;

				if (mc == null) throw new NotSupportedException();

				if (mc.Method.Name == nameof(Sql.InnerJoinTable) || mc.Method.Name == nameof(Sql.LeftJoinTable) || mc.Method.Name == nameof(Sql.CrossJoinTable))
				{
					var exp = (MethodCallExpression)expression.Arguments[0];

					// CTE - from - relation pattern

					sq = BuildRootQuery(exp, sq);

					var text = sq.ToCommand().CommandText;

					var ts = sq.GetSelectableTables().Select(x => x.Alias).ToList();
					ts.Add(joinAlias.Name!);
					sq.AddJoinClause(method, ts, joinAlias);

					if (condition != null) sq.Where(condition.ToValue(ts));

					return sq;
				}
				else if (mc.Method.Name == nameof(Sql.FromTable))
				{
					return sq;
				}
			}
		}

		if (sq.FromClause == null) throw new NotSupportedException();
		{
			var tables = sq.GetSelectableTables().Select(x => x.Alias).ToList();

			if (method != null && joinAlias != null)
			{
				tables.Add(joinAlias.Name!);
				sq.AddJoinClause(method, tables, joinAlias);
			}

			if (condition != null) sq.Where(condition.ToValue(tables));

			//refresh select clause
			if (select != null)
			{
				sq.SelectClause = null;
				sq.AddSelectClause(select, condition, tables);
			}
			return sq;
		}
	}

	private string GetTableNameOrDefault(UnaryExpression ue)
	{
		if (ue.Operand is LambdaExpression lambda)
		{
			if (lambda.Body is MethodCallExpression method)
			{
				if (method.Arguments.Count >= 1)
				{
					var val = method.Arguments[0].GetValueOrDefault();
					if (val is string s) return s;
				}
			}
		}

		return string.Empty;
	}

	private SelectQuery BuildRootQuery(MethodCallExpression expression, SelectQuery sq)
	{
		var select = expression.GetSelectLambdaFromArguments();

		if (select == null || select.Parameters.Count != 2) throw new NotSupportedException();

		var table = select.Parameters[0];
		var alias = select.Parameters[1];
		var tableName = table.Name;

		if (string.IsNullOrEmpty(table?.Name)) throw new NotSupportedException();
		if (string.IsNullOrEmpty(alias?.Name)) throw new NotSupportedException();

		var tables = new List<string> { alias.Name! };

		var v = (ValueCollection)select.Body.ToValue(tables);

		var columns = (ValueCollection)v[0];
		var columnnames = columns.Select(x => ((ColumnValue)x).Column).ToList();

		if (expression.Arguments.Count > 2 && expression.Arguments[1] is UnaryExpression ue)
		{
			var t = GetTableNameOrDefault(ue);
			if (tableName != t)
			{
				tableName = t;
				var w = sq.WithClause!.GetCommonTables().Where(x => x.Alias == tableName).First();
				columnnames = w.GetColumnNames().ToList();
			}
		}

		if (string.IsNullOrEmpty(tableName)) throw new NotSupportedException();

		var pt = new PhysicalTable()
		{
			ColumnNames = columnnames,
			Table = tableName
		};

		sq.From(pt.ToSelectable()).As(alias.Name);

		foreach (var column in columnnames)
		{
			sq.Select(alias!.Name, column);
		}

		return sq;
	}
}

internal static class ExpressionHelper
{
	internal static IEnumerable<T> GetArguments<T>(this MethodCallExpression? expression)
	{
		if (expression == null) yield break;

		for (int i = 0; i < expression.Arguments.Count(); i++)
		{
			if (expression.Arguments[i] is T exp) yield return exp;
		}
	}

	internal static T? GetArgument<T>(this MethodCallExpression? expression, int index)
	{
		if (expression == null) return default(T);

		if (expression.Arguments.Count() < index + 1) return default(T);
		if (expression.Arguments[index] is T exp) return exp;
		return default(T);
	}

	internal static T? GetOperand<T>(this UnaryExpression? expression)
	{
		if (expression == null) return default(T);

		if (expression.Operand is T exp) return exp;
		return default(T);
	}

	internal static T? GetBody<T>(this LambdaExpression? expression)
	{
		if (expression == null) return default(T);

		if (expression.Body is T exp) return exp;
		return default(T);
	}

	internal static LambdaExpression? GetSelectLambdaFromArguments(this MethodCallExpression expression)
	{
		var args = expression.GetArguments<UnaryExpression>();
		var lambdas = args.Where(x => x.Operand.NodeType == ExpressionType.Lambda).Select(x => (LambdaExpression)x.Operand);
		return lambdas.Where(x => x.Body.NodeType != ExpressionType.Call && x.ReturnType != typeof(bool)).FirstOrDefault();
	}

	internal static LambdaExpression? GetMethodLambdaFromArguments(this MethodCallExpression expression)
	{
		var args = expression.GetArguments<UnaryExpression>();
		var lambdas = args.Where(x => x.Operand.NodeType == ExpressionType.Lambda).Select(x => (LambdaExpression)x.Operand);
		return lambdas.Where(x => x.Body.NodeType == ExpressionType.Call && x.ReturnType != typeof(bool)).FirstOrDefault();
	}

	internal static LambdaExpression? GetConditionLambdaFromArguments(this MethodCallExpression expression)
	{
		var args = expression.GetArguments<UnaryExpression>();
		var lambdas = args.Where(x => x.Operand.NodeType == ExpressionType.Lambda).Select(x => (LambdaExpression)x.Operand);
		return lambdas.Where(x => x.ReturnType == typeof(bool)).FirstOrDefault();
	}
}
