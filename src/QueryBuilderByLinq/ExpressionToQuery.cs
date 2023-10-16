using Carbunql;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

public static class ExpressionToQuery
{
	public static SelectQuery ToQueryAsPostgres(this IQueryable source)
	{
		var exp = (MethodCallExpression)source.Expression;
		var builder = new SelectQueryBuilder(exp);
		return builder.Build(exp);
	}

	internal static string GetTableName(this ConstantExpression exp)
	{
		return ((IQueryable)exp.Value!).ElementType.ToTableName();
	}
}