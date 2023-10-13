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
			var builder = new SelectQueryBuilderBySelect(exp);
			return builder.Build();
		}

		if (exp.Method.Name == "Where")
		{
			var sq = CreateSelectQuery_Where(exp);
			return exp.SetSelectClause_Where(sq);
		}

		throw new NotSupportedException();
	}

	private static SelectQuery CreateSelectQuery_Where(this MethodCallExpression exp)
	{
		if (exp.Method.Name != "Where") throw new InvalidProgramException();

		var from = exp.Arguments.GetExpressions<ConstantExpression>().FirstOrDefault();
		var where = exp.Arguments.GetExpressions<UnaryExpression>().FirstOrDefault();

		if (from == null || where == null) throw new NotSupportedException();

		var table = from.GetTableName();
		var alias = where.GetTableAlias();

		var sq = new SelectQuery();
		sq.From(table).As(alias);

		var whereOperand = (LambdaExpression)where.Operand;
		sq.Where(whereOperand.ToValue(new List<string> { alias }));
		return sq;
	}

	private static SelectQuery SetSelectClause_Where(this MethodCallExpression mexp, SelectQuery sq)
	{
		var me = mexp.Arguments.GetExpressions<MethodCallExpression>().First();

		var ue = mexp.Arguments.GetExpressions<UnaryExpression>().First();
		var operand = (LambdaExpression)ue.Operand;

		var tables = sq.GetSelectableTables().Select(x => x.Alias).ToList();
		var v = operand.Body.ToValue(tables);

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
