using Carbunql;
using System.Linq.Expressions;
using System.Reflection;
using Carbunql.Building;

namespace QueryBuilderByLinq;

public static class ExpressionToQuery
{
	public static SelectQuery ToQueryAsPostgres(this Expression exp)
	{
		if (exp is MethodCallExpression mexp)
		{
			var sq = mexp.CreateSelectQueryAndSetFromClause();

			var ue = mexp.Arguments.GetExpressions<UnaryExpression>().First();
			var operand = (LambdaExpression)ue.Operand;

			var tables = sq.GetSelectableTables().Select(x => x.Alias).ToList();
			var v = operand.Body.ToValue(tables);

			sq.Select(v).As(v.GetDefaultName());

			return sq;
		}

		throw new NotImplementedException();
	}

	private static IEnumerable<T> GetExpressions<T>(this IEnumerable<Expression> expressions)
	{
		foreach (var expression in expressions)
		{
			if (expression is T result) yield return result;
		}
	}

	private static SelectQuery CreateSelectQueryAndSetFromClause(this MethodCallExpression exp)
	{
		if (exp.Method.Name != "Select") throw new InvalidProgramException();

		var me = exp.Arguments.GetExpressions<MethodCallExpression>().FirstOrDefault();

		if (me != null)
		{
			var (table, alias) = me.GetTableNameInfo();
			var sq = new SelectQuery();
			sq.From(table).As(alias);
			return sq;
		}

		var ce = exp.Arguments.GetExpressions<ConstantExpression>().FirstOrDefault();
		var ue = exp.Arguments.GetExpressions<UnaryExpression>().FirstOrDefault();

		if (ce != null && ue != null)
		{
			var table = ce.GetTableName();
			var alias = ue.GetTableAlias();

			var sq = new SelectQuery();
			sq.From(table).As(alias);
			return sq;
		}

		throw new NotSupportedException();
	}

	private static (string TableName, string Alias) GetTableNameInfo(this MethodCallExpression exp)
	{
		var tableName = ((ConstantExpression)exp.Arguments[0]).GetTableName();
		var alias = ((UnaryExpression)exp.Arguments[1]).GetTableAlias();
		return (tableName, alias);
	}

	private static string GetTableName(this ConstantExpression exp)
	{
		return ((IQueryable)exp.Value!).ElementType.ToTableName();
	}

	private static string GetTableAlias(this UnaryExpression exp)
	{
		var operand = (LambdaExpression)exp.Operand;
		return operand.Parameters[0].Name!;
	}


	private static string SetSelectClause(this UnaryExpression exp, SelectQuery sq)
	{
		var operand = (LambdaExpression)exp.Operand;
		return operand.Parameters[0].Name!;
	}
}