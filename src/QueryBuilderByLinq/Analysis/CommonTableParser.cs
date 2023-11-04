using Carbunql.Clauses;
using System.Linq.Expressions;

namespace QueryBuilderByLinq.Analysis;

public static class CommonTableParser
{
	public static List<CommonTableInfo> Parse(Expression exp)
	{
		return GetCommonTables(exp);
	}

	private static List<CommonTableInfo> GetCommonTables(Expression exp)
	{
		var lst = GetCommonTablesCore(exp);
		return lst.Reverse().ToList();
	}

	private static IEnumerable<CommonTableInfo> GetCommonTablesCore(Expression exp)
	{
		foreach (var item in exp.GetExpressions().ToList())
		{
			foreach (var ct in GetCommonTablesAsNest(item))
			{
				yield return ct;
			};
			foreach (var ct in GetCommonTablesAsRoot(item))
			{
				yield return ct;
			};
		}
	}

	private static IEnumerable<CommonTableInfo> GetCommonTablesAsNest(Expression exp)
	{
		var v = GetCommonTableAsNest(exp);
		if (v != null) yield return v;
	}

	private static IEnumerable<CommonTableInfo> GetCommonTablesAsRoot(Expression exp)
	{
		var v = GetCommmonTableAsRoot(exp);
		if (v != null) yield return v;
	}

	private static CommonTableInfo? GetCommmonTableAsRoot(Expression exp)
	{
		/*
			Query = MethodCallExpression.Argument[0]
			Name  = MethodCallExpression.Argument[1].Operand.Body.Parameters[0].Name					
		 */
		if (exp is not MethodCallExpression) return null;

		var method = (MethodCallExpression)exp;
		if (method.Arguments.Count < 2) return null;

		var commonTableQuery = method.GetArgument<ConstantExpression>(0);
		if (commonTableQuery == null) return null;

		var operand = method.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>();
		if (operand == null) return null;
		if (operand.GetBody<MethodCallExpression>()?.Method.Name != nameof(CommonTable)) return null;
		if (operand.Parameters.Count != 1) return null;
		var name = operand.Parameters[0].Name!;

		if (Queryable.TryParse(commonTableQuery, out var query))
		{
			return new CommonTableInfo(query, name);
		}

		return null;
	}

	private static CommonTableInfo? GetCommonTableAsNest(Expression exp)
	{
		/*
			Query = MethodCallExpression.Argument[1].Operand.Body
			Name  = MethodCallExpression.Argument[2].Operand.Parameters[1].Name					
		 */
		if (exp is not MethodCallExpression) return null;

		var root = (MethodCallExpression)exp;
		if (root.Arguments.Count < 3) return null;

		var commonTable = root.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>().GetBody<MethodCallExpression>();
		if (commonTable == null || commonTable.Method.Name != nameof(CommonTable)) return null;

		var parameter = root.GetArgument<UnaryExpression>(2).GetOperand<LambdaExpression>().GetParameter<ParameterExpression>(1);
		if (parameter == null) return null;

		if (Queryable.TryParse(commonTable, out var query))
		{
			return new CommonTableInfo(query, parameter.Name!);
		}

		return null;
	}
}
