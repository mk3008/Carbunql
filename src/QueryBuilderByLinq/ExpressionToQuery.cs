using Carbunql;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

public static class ExpressionToQuery
{
	public static SelectQuery ToQueryAsPostgres(this IQueryable source)
	{
		var exp = (MethodCallExpression)source.Expression;
		return exp.CreateSelectQuery();
	}

	//private static SelectQuery ToQueryAsPostgres(this Expression exp)
	//{
	//	if (exp is MethodCallExpression mexp)
	//	{
	//		var sq = mexp.CreateSelectQuery();
	//		return sq;
	//	}

	//	throw new NotImplementedException();
	//}

	//internal static IEnumerable<T> GetExpressions<T>(this IEnumerable<Expression> expressions)
	//{
	//	foreach (var expression in expressions)
	//	{
	//		if (expression is T result) yield return result;
	//	}
	//}

	internal static string GetTableName(this ConstantExpression exp)
	{
		return ((IQueryable)exp.Value!).ElementType.ToTableName();
	}

	//internal static string SetSelectClause(this UnaryExpression exp, SelectQuery sq)
	//{
	//	var operand = (LambdaExpression)exp.Operand;
	//	return operand.Parameters[0].Name!;
	//}
}