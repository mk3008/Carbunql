using Carbunql.Clauses;
using System.Linq.Expressions;

namespace QueryBuilderByLinq.Analysis;

public static class CommonTableInfoParser
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
			if (TryParseAsNestedCommonTable(item, out var info))
			{
				yield return info;
			}
		}
		foreach (var item in exp.GetExpressions().ToList())
		{
			if (TryParseAsRootCommonTable(item, out var info))
			{
				yield return info;
			}
		}
	}

	private static bool TryParseAsRootCommonTable(Expression exp, out CommonTableInfo info)
	{
		info = null!;

		/*
			Query = MethodCallExpression.Argument[0]
			Name  = MethodCallExpression.Argument[1].Operand.Body.Parameters[0].Name					
		 */
		if (exp is not MethodCallExpression) return false;

		var method = (MethodCallExpression)exp;
		if (method.Arguments.Count != 3) return false;

		var commonTableQuery = method.GetArgument<ConstantExpression>(0);
		if (commonTableQuery == null) return false;

		var operand = method.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>();
		if (operand == null) return false;

		var body = operand.GetBody<MethodCallExpression>();
		if (body == null) return false;

		if (body.Method.Name == nameof(Sql.FromTable))
		{
			if (operand.Parameters.Count != 1) return false;
			var name = operand.Parameters[0].Name!;

			var c = method.GetArgument<ConstantExpression>(0);
			if (c?.Value is TableQuery tq)
			{
				info = new CommonTableInfo(tq.InnerQuery!.AsQueryable(), name);
				return true;
			}
		}
		else if (body.Method.Name == nameof(CommonTable))
		{
			if (operand.Parameters.Count != 1) throw new NotSupportedException();

			var name = operand.Parameters[0].Name!;

			if (Queryable.TryParse(commonTableQuery, out var query))
			{
				info = new CommonTableInfo(query, name);
				return true;
			}
		}

		throw new NotSupportedException();
	}

	private static bool TryParseAsNestedCommonTable(Expression exp, out CommonTableInfo info)
	{
		info = null!;

		/*
			arg1 Query
			arg2 Parameter of alias name.
		 */
		if (exp is not MethodCallExpression) return false;

		var root = (MethodCallExpression)exp;
		if (root.Arguments.Count != 3) return false;

		var commonTable = root.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>().GetBody<MethodCallExpression>();
		if (commonTable == null || commonTable.Method.Name != nameof(CommonTable)) return false;

		var parameter = root.GetArgument<UnaryExpression>(2).GetOperand<LambdaExpression>().GetParameter<ParameterExpression>(1);
		if (parameter == null) return false;

		if (Queryable.TryParse(commonTable, out var query))
		{
			info = new CommonTableInfo(query, parameter.Name!);
			return true;
		}

		throw new NotSupportedException();
	}
}
