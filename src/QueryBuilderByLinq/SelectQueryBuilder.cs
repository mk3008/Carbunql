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

			if (sq.FromClause == null)
			{
				sq = BuildRootOrNestedQuery(expression, sq);
			}
			else
			{
				sq = BuildNestedQuery(expression, sq);
			}
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
		if (root != null && condition == null && method != null && method.Body is MethodCallExpression nestBody && select != null)
		{
			if (nestBody.Method.Name == nameof(Sql.FromTable))
			{
				// CTE, From pattern
				var sq = BuildCteQuery(expression, root, method, select);
				// from
				sq.AddRootQuery(expression);
				return sq;
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


	private ParameterExpression? GetJoinParameter(LambdaExpression? select, LambdaExpression? where)
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

	private SelectQuery BuildRootOrNestedQuery(MethodCallExpression expression, SelectQuery sq)
	{
		if (sq.FromClause != null) throw new Exception();

		var condition = expression.GetConditionLambdaFromArguments();
		var method = expression.GetMethodLambdaFromArguments();
		var select = expression.GetSelectLambdaFromArguments();

		if (method == null)
		{
			var exp = (MethodCallExpression)expression.Arguments[0];
			sq.AddRootQuery(exp);

			var tables = sq.GetSelectableTables().Select(x => x.Alias).ToList();
			if (condition != null) sq.Where(condition.ToValue(tables));

			return sq;
		}

		var mc = method.GetBody<MethodCallExpression>()!;

		if (mc.Method.Name == nameof(Sql.FromTable))
		{
			return sq;
		}
		else if (mc.Method.Name == nameof(Sql.InnerJoinTable) || mc.Method.Name == nameof(Sql.LeftJoinTable) || mc.Method.Name == nameof(Sql.CrossJoinTable))
		{
			var joinParam = GetJoinParameter(select, condition);
			if (joinParam == null) throw new Exception();

			var exp = (MethodCallExpression)expression.Arguments[0];

			// CTE - from - relation pattern

			sq.AddRootQuery(exp);

			var ts = sq.GetSelectableTables().Select(x => x.Alias).ToList();
			ts.Add(joinParam.Name!);
			sq.AddJoinClause(method, ts, joinParam);

			if (condition != null) sq.Where(condition.ToValue(ts));

			return sq;
		}
		else if (select != null && mc.Method.Name == nameof(Sql.CommonTable))
		{
			// add CommonTable
			var alias = select.Parameters.Last();

			var body = method.GetBody<MethodCallExpression>()!;
			if (Queryable.TryParse(body, out var cte))
			{
				sq.With(cte.ToQueryAsPostgres()).As(alias!.Name!);
				return sq;
			}
		}

		throw new Exception();
	}

	private SelectQuery BuildNestedQuery(MethodCallExpression expression, SelectQuery sq)
	{
		if (sq.FromClause == null) throw new NotSupportedException();

		var condition = expression.GetConditionLambdaFromArguments();
		var method = expression.GetMethodLambdaFromArguments();
		var select = expression.GetSelectLambdaFromArguments();
		var joinAlias = GetJoinParameter(select, condition);

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

	internal static List<string> DecodeColumnNames(this ValueCollection collection, string tableAlias)
	{
		var columnNames = new List<string>();
		foreach (var item in collection)
		{
			if (item is ValueCollection vc)
			{
				columnNames.AddRange(vc.DecodeColumnNames(tableAlias));
			}
			else if (item is ColumnValue c && c.TableAlias == tableAlias)
			{
				columnNames.Add(c.Column);
			}
		}
		return columnNames;
	}

	internal static SelectQuery AddRootQuery(this SelectQuery sq, MethodCallExpression expression)
	{
		var select = expression.GetSelectLambdaFromArguments();
		var method = expression.GetMethodLambdaFromArguments();

		if (select == null || select.Parameters.Count != 2) throw new NotSupportedException();

		var table = select.Parameters[0];
		var alias = select.Parameters[1];
		var tableName = table.Name;

		if (string.IsNullOrEmpty(table?.Name)) throw new NotSupportedException();
		if (string.IsNullOrEmpty(alias?.Name)) throw new NotSupportedException();

		var tables = new List<string> { alias.Name! };

		var v = (ValueCollection)select.Body.ToValue(tables);

		var columnNames = v.DecodeColumnNames(alias.Name!);

		if (method != null)
		{
			var name = expression.GetArgument<MethodCallExpression>(index: 0).GetArgument<MethodCallExpression>(index: 0).GetArgument<UnaryExpression>(index: 1).GetOperand<LambdaExpression>()?.Parameters[0].Name;
			if (name == null) name = expression.GetArgument<UnaryExpression>(index: 1).GetOperand<LambdaExpression>().GetBody<MethodCallExpression>().GetArgument<MemberExpression>(index: 0)?.Member.Name;

			// select CTE
			if (name != null && tableName != name)
			{
				tableName = name;
				var w = sq.WithClause!.GetCommonTables().Where(x => x.Alias == tableName).First();
				columnNames = w.GetColumnNames().ToList();
			}
		}

		if (string.IsNullOrEmpty(tableName)) throw new NotSupportedException();

		var pt = new PhysicalTable()
		{
			ColumnNames = columnNames,
			Table = tableName
		};

		sq.From(pt.ToSelectable()).As(alias.Name);

		foreach (var column in columnNames)
		{
			sq.Select(alias!.Name, column);
		}

		return sq;
	}

	internal static SelectQuery AddCommonTable(this SelectQuery sq, MethodCallExpression expression)
	{
		var select = expression.GetSelectLambdaFromArguments();
		var method = expression.GetMethodLambdaFromArguments();

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

		if (method != null)
		{
			var t = method.GetBody<MethodCallExpression>().GetArgument<ConstantExpression>(index: 0)!.Value!.ToString();

			// select CTE
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
