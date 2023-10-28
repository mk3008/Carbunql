using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Values;
using System.Collections;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

internal static class ExpressionExtension
{


	internal static object? GetValueOrDefault(this Expression exp)
	{
		if (exp is ConstantExpression m)
		{
			return m.Value;
		}
		return null;
	}

	internal static ValueBase ToValue(this Expression exp, List<string> tables)
	{
		if (exp.NodeType == ExpressionType.Constant)
		{
			return ((ConstantExpression)exp).ToValue();
		}

		if (exp.NodeType == ExpressionType.MemberAccess)
		{
			return ((MemberExpression)exp).ToValue(tables);
		}

		if (exp is UnaryExpression unary)
		{
			return unary.ToValue(tables);
		}

		if (exp is NewExpression ne)
		{
			return ne.ToValue(tables);
		}

		if (exp.NodeType == ExpressionType.Invoke)
		{
			return ((InvocationExpression)exp).ToParameterValue();
		}

		if (exp.NodeType == ExpressionType.Call)
		{
			var mc = (MethodCallExpression)exp;

			if (mc.Method.DeclaringType == typeof(Sql))
			{
				//if (mc.Method.Name == "ExistsAs") return mc.ToExistsExpression(tables);
				//if (mc.Method.Name == "InAs") return mc.ToInClause(tables);
				//if (mc.Method.Name == "RowNumber") return mc.ToFunctionValue(tables);
				return mc.ToFunctionValue(tables);
			}

			if (mc.Method.Name == "Concat") return mc.ToConcatValue(tables);

			if (mc.Method.Name == "Contains")
			{
				if (mc.Object == null && mc.Method.DeclaringType == typeof(Enumerable))
				{
					return mc.ToAnyFunctionValue(tables);
				}
				if (mc.Object != null && typeof(IList).IsAssignableFrom(mc.Object.Type))
				{
					return mc.ToAnyFunctionValue(tables);
				}
				else
				{
					return mc.ToContainsLikeClause(tables);
				}
			}

			if (mc.Method.DeclaringType == typeof(Convert))
			{
				return mc.ToCastValue(tables);
			}

			if (mc.Object == null) throw new NullReferenceException("MethodCallExpression.Object is null.");
			if (mc.Object.NodeType == ExpressionType.Constant)
			{
				return ((MethodCallExpression)exp).ToParameterValue();
			}

			if (mc.Method.Name == "StartsWith") return mc.ToStartsWithLikeClause(tables);
			if (mc.Method.Name == "EndsWith") return mc.ToEndsWithLikeClause(tables);
			if (mc.Method.Name == "Trim") return mc.ToTrimValue(tables);
			if (mc.Method.Name == "TrimStart") return mc.ToTrimStartValue(tables);
			if (mc.Method.Name == "TrimEnd") return mc.ToTrimEndValue(tables);

			if (mc.Method.Name == nameof(String.ToString)) return mc.ToStringValue(tables);

			return ((MethodCallExpression)exp).ToParameterValue();
		}

		if (exp is NewArrayExpression arrayexp)
		{
			return arrayexp.ToValue(tables);
		}

		if (exp is ConditionalExpression cond)
		{
			return cond.ToValue(tables);
		}

		if (exp is ParameterExpression pexp)
		{
			var alias = pexp.Name!;
			var lst = pexp.Type.GetProperties().Select(x => new ColumnValue(alias, x.Name)).ToList<ValueBase>();
			return new ValueCollection(lst);
		}

		if (exp is BinaryExpression bexp)
		{
			return bexp.ToValueExpression(tables);
		}

		if (exp is LambdaExpression lexp)
		{
			return lexp.Body.ToValue(tables);
		}

		throw new NotSupportedException(exp.GetType().Name);
	}

	internal static object? Execute(this Expression exp)
	{
		var m = typeof(IQueryableExtension).GetMethod(nameof(ExecuteCore));
		if (m != null)
		{
			var method = m.MakeGenericMethod(exp.Type);
			return method.Invoke(null, new object[] { exp });
		}

		throw new NotSupportedException();
	}

	private static T ExecuteCore<T>(this Expression exp)
	{
		var lm = Expression.Lambda<Func<T>>(exp);
		return lm.Compile().Invoke();
	}
}
