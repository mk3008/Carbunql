using Carbunql;
using QueryBuilderByLinq.Analysis;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

public static class IQueryableExtension
{
	public static SelectQuery ToQueryAsPostgres(this IQueryable source)
	{
		var exp = (MethodCallExpression)source.Expression;
		//ExpressionDebugger.WriteImmediate(exp);
		var ctes = CommonTableInfoParser.Parse(exp).ToList();
		var from = FromTableInfoParser.Parse(exp);

		var builder = new SelectQueryBuilder(exp);
		return builder.Build(exp);
	}
}