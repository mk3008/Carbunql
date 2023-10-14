using Carbunql;
using Carbunql.Building;
using Carbunql.Values;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

internal static class MethodCallExpressionExtension
{
	internal static SelectQuery CreateSelectQuery(this MethodCallExpression exp)
	{
		if (exp.Method.Name == "Select")
		{
			//var builder = new SelectQueryBuilderBySelect(exp);
			//return builder.Build();
			var builder = new SelectQueryBuilderBySelectMany(exp);
			return builder.Build(exp);
		}

		if (exp.Method.Name == "Where")
		{
			var builder = new SelectQueryBuilderByWhere(exp);
			return builder.Build();
		}

		if (exp.Method.Name == "Join")
		{
			var builder = new SelectQueryBuilderByJoin(exp);
			return builder.Build();
		}

		if (exp.Method.Name == "SelectMany")
		{
			var builder = new SelectQueryBuilderBySelectMany(exp);
			return builder.Build(exp);
		}


		throw new NotSupportedException($"Method not supported: {exp.Method.Name}");
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

public class SelectQueryBuilderBySelect
{
	public SelectQueryBuilderBySelect(MethodCallExpression expression)
	{
		if (expression.Method.Name != "Select") throw new InvalidProgramException();
		Expression = expression;
	}

	public MethodCallExpression Expression { get; init; }

	public SelectQuery Build()
	{
		var selectwhere = Expression.Arguments.GetExpressions<MethodCallExpression>().FirstOrDefault();

		if (selectwhere != null)
		{
			return Build(selectwhere);
		}

		var table = Expression.Arguments.GetExpressions<ConstantExpression>().FirstOrDefault();
		var select = Expression.Arguments.GetExpressions<UnaryExpression>().FirstOrDefault();

		if (table == null || select == null) throw new NotSupportedException();

		return Build(table, select);
	}

	private SelectQuery Build(MethodCallExpression selectwhere)
	{
		if (selectwhere.Method.Name != "Where") throw new InvalidProgramException();

		var (table, alias) = selectwhere.GetTableNameInfo();
		var sq = new SelectQuery();
		sq.From(table).As(alias);

		var where = Expression.Arguments.GetExpressions<MethodCallExpression>().First();
		sq = SetWhereClause(sq, where);

		var select = Expression.Arguments.GetExpressions<UnaryExpression>().First();
		sq = SetSelectClause(sq, select);

		return sq;
	}

	private SelectQuery Build(ConstantExpression table, UnaryExpression select)
	{
		var name = table.GetTableName();
		var alias = select.GetTableAlias();

		var sq = new SelectQuery();
		sq.From(name).As(alias);
		sq = SetSelectClause(sq, select);

		return sq;
	}

	private SelectQuery SetWhereClause(SelectQuery sq, MethodCallExpression where)
	{
		var unary = where.Arguments.GetExpressions<UnaryExpression>().First();

		var tables = sq.GetSelectableTables().Select(x => x.Alias).ToList();
		sq.Where(unary.Operand.ToValue(tables));

		return sq;
	}

	private SelectQuery SetSelectClause(SelectQuery sq, UnaryExpression select)
	{
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

public class SelectQueryBuilderByWhere
{
	public SelectQueryBuilderByWhere(MethodCallExpression expression)
	{
		if (expression.Method.Name != "Where") throw new InvalidProgramException();
		Expression = expression;
	}

	public MethodCallExpression Expression { get; init; }

	public SelectQuery Build()
	{
		var from = Expression.Arguments.GetExpressions<ConstantExpression>().FirstOrDefault();
		var where = Expression.Arguments.GetExpressions<UnaryExpression>().FirstOrDefault();

		if (from == null || where == null) throw new NotSupportedException();

		var name = from.GetTableName();
		var alias = where.GetTableAlias();

		var sq = new SelectQuery();
		sq.From(name).As(alias);

		SetWhereClause(sq, where);
		SetSelectClause(sq, where);

		return sq;
	}

	private SelectQuery SetWhereClause(SelectQuery sq, UnaryExpression where)
	{
		var tables = sq.GetSelectableTables().Select(x => x.Alias).ToList();
		sq.Where(where.Operand.ToValue(tables));

		return sq;
	}

	private SelectQuery SetSelectClause(SelectQuery sq, UnaryExpression where)
	{
		var operand = (LambdaExpression)where.Operand;
		var tp = operand.Parameters[0].Type;
		var alias = operand.Parameters[0].Name!;
		tp.GetProperties().ToList().ForEach(x =>
		{
			sq.Select(alias, x.Name).As(x.Name);
		});

		return sq;
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

public class SelectQueryBuilderBySelectMany
{
	public SelectQueryBuilderBySelectMany(MethodCallExpression expression)
	{
		//if (expression.Method.Name != "SelectMany") throw new InvalidProgramException();
		Expression = expression;
	}

	public MethodCallExpression Expression { get; init; }

	private LambdaExpression GetSelectExpression(MethodCallExpression expression)
	{
		if (expression.Arguments.Count == 2)
		{
			var ue = (UnaryExpression)expression.Arguments[1];
			return (LambdaExpression)ue.Operand;
		}
		else if (expression.Arguments.Count == 3)
		{
			var ue = (UnaryExpression)expression.Arguments[2];
			return (LambdaExpression)ue.Operand;
		}
		throw new NotSupportedException();
		//var q = expression.Arguments.GetExpressions<UnaryExpression>();
		//return q.Select(x => x.Operand).GetExpressions<LambdaExpression>().Where(x => x.ReturnType.IsAnonymousType()).First();
	}

	private LambdaExpression? GetJoinExpression(MethodCallExpression expression)
	{
		if (expression.Arguments.Count == 3)
		{
			var ue = (UnaryExpression)expression.Arguments[1];
			return (LambdaExpression)ue.Operand;
		}
		return null;
		//var q = expression.Arguments.GetExpressions<UnaryExpression>();
		//return q.Select(x => x.Operand).GetExpressions<LambdaExpression>().Where(x => !x.ReturnType.IsAnonymousType()).FirstOrDefault();
	}

	//private ConstantExpression GetFromExpression()
	//{
	//	var exp = Expression.Arguments[0];
	//	if (exp is ConstantExpression ce) return ce;

	//	if (exp is MethodCallExpression me)
	//	{
	//		var arg0 = (MethodCallExpression)me.Arguments[0];
	//		var arg00 = (MethodCallExpression)arg0.Arguments[0];
	//	}

	//	throw new NotSupportedException();
	//}

	//private ConstantExpression GetFromExpression(MethodCallExpression expression)
	//{
	//	var arg0 = expression.Arguments[0];
	//	if (arg0 is ConstantExpression ce)
	//	{

	//	}

	//	if (arg0 is MethodCallExpression me)
	//	{
	//		return GetFromExpression(me);
	//	}

	//	throw new NotSupportedException();
	//}

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

		var fromTableAlias = select.Parameters.First().Name!;
		var tables = select.Parameters.Select(x => x.Name!).ToList();

		var sq = new SelectQuery();
		var (f, t) = sq.From(from.GetTableName()).As(fromTableAlias);

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