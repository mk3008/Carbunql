using System.Linq.Expressions;

namespace Carbunql.Postgres.Analysis;

internal static class ExpressionHelper
{
	internal static IEnumerable<T> GetArguments<T>(this MethodCallExpression? expression)
	{
		if (expression == null) yield break;

		for (int i = 0; i < expression.Arguments.Count(); i++)
		{
			if (expression.Arguments[i] is T exp) yield return exp;
		}
	}

	internal static T? GetArgument<T>(this MethodCallExpression? expression, int index)
	{
		if (expression == null) return default(T);

		if (expression.Arguments.Count() < index + 1) return default(T);
		if (expression.Arguments[index] is T exp) return exp;
		return default(T);
	}

	internal static T? GetParameter<T>(this LambdaExpression? expression, int index)
	{
		if (expression == null) return default(T);

		if (expression.Parameters.Count() < index + 1) return default(T);
		if (expression.Parameters[index] is T exp) return exp;
		return default(T);
	}

	internal static T? GetOperand<T>(this UnaryExpression? expression)
	{
		if (expression == null) return default(T);

		if (expression.Operand is T exp) return exp;
		return default(T);
	}

	internal static T? GetBody<T>(this LambdaExpression? expression)
	{
		if (expression == null) return default(T);

		if (expression.Body is T exp) return exp;
		return default(T);
	}
}