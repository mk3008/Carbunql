using Carbunql.Clauses;
using System.Linq.Expressions;

namespace QueryBuilderByLinq.Analysis;

public class WhereValueParser
{
	public static ValueBase? Parse(Expression exp)
	{
		var tableinfo = TableInfoParser.Parse(exp);
		var joinInfos = JoinTableInfoParser.Parse(exp);

		var aliases = new List<string>();
		if (tableinfo != null) aliases.Add(tableinfo.Alias);
		foreach (var item in joinInfos)
		{
			aliases.Add(item.TableInfo.Alias);
		}

		return Parse(exp, aliases);
	}

	public static ValueBase? Parse(Expression exp, List<string> tableAliases)
	{
		if (TryParseAsRoot(exp, out var root)) return root;
		if (TryParseAsNest(exp, tableAliases, out var nest)) return nest;
		return null;
	}

	public static bool TryParseAsRoot(Expression exp, out ValueBase value)
	{
		value = null!;

		if (exp is not MethodCallExpression) return false;
		var root = (MethodCallExpression)exp;

		if (root.Arguments.Count != 2) return false;
		var operand = root.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>();
		if (operand == null || operand.ReturnType != typeof(bool)) return false;

		value = operand.Body.ToValue();
		return true;
	}

	public static bool TryParseAsNest(Expression exp, List<string> aliases, out ValueBase value)
	{
		value = null!;

		if (exp is not MethodCallExpression) return false;
		var root = (MethodCallExpression)exp;
		if (root.Arguments.Count != 2) return false;

		var method = root.GetArgument<MethodCallExpression>(0);
		if (method == null) return false;

		return TryParseAsNestCore(method, aliases, out value);
	}

	private static bool TryParseAsNestCore(Expression exp, List<string> aliases, out ValueBase value)
	{
		value = null!;

		if (exp is not MethodCallExpression) return false;
		var root = (MethodCallExpression)exp;

		if (root.Arguments.Count != 2) return false;
		var operand = root.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>();
		if (operand == null || operand.ReturnType != typeof(bool)) return false;

		value = operand.Body.ToValue(aliases);
		return true;
	}
}
