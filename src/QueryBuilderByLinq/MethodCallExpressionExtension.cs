using Carbunql;
using Carbunql.Building;
using Carbunql.Values;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

internal static class MethodCallExpressionExtension
{
	internal static SelectQuery CreateSelectQuery(this MethodCallExpression exp)
	{
		var builder = new SelectQueryBuilder(exp);
		return builder.Build(exp);
	}

	internal static (string TableName, string Alias) GetTableNameInfo(this MethodCallExpression exp)
	{
		var tableName = ((ConstantExpression)exp.Arguments[0]).GetTableName();
		var alias = ((UnaryExpression)exp.Arguments[1]).GetTableAlias();
		return (tableName, alias);
	}

	internal static string GetTableAlias(this UnaryExpression exp)
	{
		var operand = (LambdaExpression)exp.Operand;
		return operand.Parameters[0].Name!;
	}
}

public class SelectQueryBuilderByJoin
{
	public SelectQueryBuilderByJoin(MethodCallExpression expression)
	{
		if (expression.Method.Name != "Join") throw new InvalidProgramException();
		Expression = expression;
	}

	public MethodCallExpression Expression { get; init; }

	public SelectQuery Build()
	{
		var from = (ConstantExpression)Expression.Arguments[0];
		var to = (ConstantExpression)Expression.Arguments[1];

		var fromCondition = (UnaryExpression)Expression.Arguments[2];
		var toCondition = (UnaryExpression)Expression.Arguments[3];

		var select = (UnaryExpression)Expression.Arguments[4];
		var selectLambda = (LambdaExpression)select.Operand;

		var tables = new List<string>()
		{
			fromCondition.GetTableAlias(),
			toCondition.GetTableAlias()
		};

		var sq = new SelectQuery();
		var (f, t) = sq.From(from.GetTableName()).As(fromCondition.GetTableAlias());
		f.InnerJoin(to.GetTableName()).As(toCondition.GetTableAlias()).On(jt =>
		{
			var left = ((LambdaExpression)fromCondition.Operand).Body.ToValue(tables);
			var right = ((LambdaExpression)toCondition.Operand).Body.ToValue(tables);
			return left.Equal(right);
		});

		SetSelectClause(sq);

		return sq;
	}

	private SelectQuery SetSelectClause(SelectQuery sq)
	{
		var select = (UnaryExpression)Expression.Arguments[4];
		var lambda = (LambdaExpression)select.Operand;

		var tables = sq.GetSelectableTables().Select(x => x.Alias).ToList();
		var v = lambda.Body.ToValue(tables);

		if (v is ValueCollection vc)
		{
			foreach (var item in vc)
			{
				sq.Select(item).As(!string.IsNullOrEmpty(item.RecommendedName) ? item.RecommendedName : item.GetDefaultName());
			}
		}
		else
		{
			sq.Select(v).As(!string.IsNullOrEmpty(v.RecommendedName) ? v.RecommendedName : v.GetDefaultName());
		}

		return sq;
	}
}

public class RootTable
{
	public RootTable(ConstantExpression from, string table, string alias)
	{
		From = from;
		Table = table;
		Alias = alias;
	}
	public ConstantExpression From { get; init; }
	public string Table { get; set; }
	public string Alias { get; set; }
}

public class SelectQueryBuilder
{
	public SelectQueryBuilder(MethodCallExpression expression)
	{
		//if (expression.Method.Name != "SelectMany") throw new InvalidProgramException();
		Expression = expression;
	}

	public MethodCallExpression Expression { get; init; }

	private LambdaExpression? GetSelectExpression(MethodCallExpression expression)
	{
		if (expression.Arguments.Count == 2)
		{
			var ue = (UnaryExpression)expression.Arguments[1];
			var operand = (LambdaExpression)ue.Operand;
			if (operand.ReturnType == typeof(bool)) return null;
			return operand;
		}
		else if (expression.Arguments.Count == 3)
		{
			var ue = (UnaryExpression)expression.Arguments[2];
			return (LambdaExpression)ue.Operand;
		}
		throw new NotSupportedException();
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
			return BuildAsRoot(expression);
		}
		else if (expression.Arguments[0] is MethodCallExpression mce)
		{
			var sq = Build(mce);
			var x = BuildAsNest(expression, sq);
			return sq;
		}

		throw new NotSupportedException();
	}

	private SelectQuery BuildAsRoot(MethodCallExpression expression)
	{
		var from = (ConstantExpression)expression.Arguments[0];
		var join = GetJoinExpression(expression);
		var select = GetSelectExpression(expression);
		var where = GetWhereExpression(expression);

		var fromTableAlias = string.Empty;
		if (select != null)
		{
			fromTableAlias = select.Parameters.First().Name!;
		}
		else if (where != null)
		{
			fromTableAlias = where.Parameters.First().Name!;
		}
		else
		{
			throw new NotSupportedException();
		}

		var tables = (select == null) ? new List<string> { fromTableAlias } : select.Parameters.Select(x => x.Name!).ToList();

		var sq = new SelectQuery();
		var (f, t) = sq.From(from.GetTableName()).As(fromTableAlias);

		if (where != null) sq.Where(where.ToValue(tables));

		if (join != null)
		{
			var me = (MethodCallExpression)join.Body;

			if (me.Method.Name == "InnerJoin")
			{
				var arg = (UnaryExpression)me.Arguments.First();
				var lambda = (LambdaExpression)arg.Operand;

				var table = lambda.Parameters.First();
				var alias = table.Name!;
				var tablename = table.Type.ToTableName();

				var condition = lambda.ToValue(tables);
				f.InnerJoin(tablename).As(alias).On((_) => condition);
			}
			else if (me.Method.Name == "LeftJoin")
			{
				var arg = (UnaryExpression)me.Arguments.First();
				var lambda = (LambdaExpression)arg.Operand;

				var table = lambda.Parameters.First();
				var alias = table.Name!;
				var tablename = table.Type.ToTableName();

				var condition = lambda.ToValue(tables);
				f.LeftJoin(tablename).As(alias).On((_) => condition);
			}
			//else if (me.Method.Name == "From")
			//{
			//	var table = item.Parameters.First();
			//	var alias = table.Name!;
			//	var tablename = table.Type.ToTableName();

			//	f.CrossJoin(tablename).As(alias);
			//}
			else
			{
				throw new NotSupportedException();
			}
		}

		if (select != null)
		{
			var v = select.Body.ToValue(tables);

			if (v is ValueCollection vc)
			{
				foreach (var item in vc)
				{
					sq.Select(item).As(!string.IsNullOrEmpty(item.RecommendedName) ? item.RecommendedName : item.GetDefaultName());
				}
			}
			else
			{
				sq.Select(v).As(!string.IsNullOrEmpty(v.RecommendedName) ? v.RecommendedName : v.GetDefaultName());
			}
		}
		else if (where != null)
		{
			var tp = where.Parameters[0].Type;
			var alias = where.Parameters[0].Name!;
			tp.GetProperties().ToList().ForEach(x =>
			{
				sq.Select(alias, x.Name).As(x.Name);
			});
		}
		else
		{
			throw new NotSupportedException();
		}

		return sq;
	}

	private SelectQuery BuildAsNest(MethodCallExpression expression, SelectQuery sq)
	{
		sq.SelectClause = null;

		var join = GetJoinExpression(expression);
		var select = GetSelectExpression(expression);

		var joinAlias = select.Parameters[1].Name!;
		var tables = sq.GetSelectableTables().Select(x => x.Alias).ToList();
		tables.Add(joinAlias);

		var f = sq.FromClause!;

		if (join != null)
		{
			var me = (MethodCallExpression)join.Body;

			if (me.Method.Name == "InnerJoin")
			{
				var arg = (UnaryExpression)me.Arguments.First();
				var lambda = (LambdaExpression)arg.Operand;

				var table = lambda.Parameters.First();
				var alias = table.Name!;
				var tablename = table.Type.ToTableName();

				var condition = lambda.ToValue(tables);
				f.InnerJoin(tablename).As(alias).On((_) => condition);
			}
			else if (me.Method.Name == "LeftJoin")
			{
				var arg = (UnaryExpression)me.Arguments.First();
				var lambda = (LambdaExpression)arg.Operand;

				var table = lambda.Parameters.First();
				var alias = table.Name!;
				var tablename = table.Type.ToTableName();

				var condition = lambda.ToValue(tables);
				f.LeftJoin(tablename).As(alias).On((_) => condition);
			}
			//else if (me.Method.Name == "From")
			//{
			//	var table = item.Parameters.First();
			//	var alias = table.Name!;
			//	var tablename = table.Type.ToTableName();

			//	f.CrossJoin(tablename).As(alias);
			//}
			else
			{
				throw new NotSupportedException();
			}
		}

		var v = select.Body.ToValue(tables);

		if (v is ValueCollection vc)
		{
			foreach (var item in vc)
			{
				sq.Select(item).As(!string.IsNullOrEmpty(item.RecommendedName) ? item.RecommendedName : item.GetDefaultName());
			}
		}
		else
		{
			sq.Select(v).As(!string.IsNullOrEmpty(v.RecommendedName) ? v.RecommendedName : v.GetDefaultName());
		}

		return sq;
	}
}