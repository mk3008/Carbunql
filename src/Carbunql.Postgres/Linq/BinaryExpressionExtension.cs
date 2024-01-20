using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Postgres;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Postgres.Linq;

internal static class BinaryExpressionExtension
{
	internal static ValueBase ToValueExpression(this BinaryExpression exp, List<string> tables)
	{
		var op = string.Empty;
		switch (exp.NodeType)
		{
			case ExpressionType.Add:
				op = "+";
				break;

			case ExpressionType.AndAlso:
				op = "and";
				break;

			case ExpressionType.Equal:
				op = "=";
				break;

			case ExpressionType.Divide:
				op = "/";
				break;

			case ExpressionType.GreaterThan:
				op = ">";
				break;

			case ExpressionType.GreaterThanOrEqual:
				op = ">=";
				break;

			case ExpressionType.LessThan:
				op = "<";
				break;

			case ExpressionType.LessThanOrEqual:
				op = "<=";
				break;

			case ExpressionType.OrElse:
				op = "or";
				break;

			case ExpressionType.NotEqual:
				op = "<>";
				break;

			case ExpressionType.Multiply:
				op = "*";
				break;

			case ExpressionType.Subtract:
				op = "-";
				break;

			case ExpressionType.Coalesce:
				return exp.ToCoalesceValue(tables);

			default:
				break;
		}

		if (string.IsNullOrEmpty(op))
		{
			throw new NotSupportedException($"NodeType:{exp.NodeType}");
		}

		var isBracket = exp.ToString().StartsWith("(");

		var left = exp.Left.ToValue(tables);
		var right = exp.Right.ToValue(tables);

		if (left is LiteralValue lv && lv.IsNullValue || right is LiteralValue rv && rv.IsNullValue)
		{
			op = op == "=" ? "is" : "is not";
		}
		else if (op == "+" && (exp.Left.Type == typeof(string) || exp.Right.Type == typeof(string)))
		{
			op = "||";
		}

		if (!op.IsEqualNoCase("or") && !op.IsEqualNoCase("and"))
		{
			left.AddOperatableValue(op, right);
			return left;
		}

		if (left is BracketValue bv)
		{
			// if logical operators are the same, do not nest brackets
			var operators = bv.Inner.GetOperators().Select(x => x.ToLower()).Where(x => x == "or" || x == "and").Distinct().ToList();
			if (operators.Count == 1 && operators.First().IsEqualNoCase(op))
			{
				bv.Inner.AddOperatableValue(op, right);
				return left;
			}
		}

		left.AddOperatableValue(op, right);
		return new BracketValue(left);
	}

	private static FunctionValue ToCoalesceValue(this BinaryExpression exp, List<string> tables)
	{
		var vc = exp.ToValueCollection(tables);

		return new FunctionValue("coalesce", vc);
	}

	private static ValueCollection ToValueCollection(this BinaryExpression exp, List<string> tables)
	{
		var right = exp.Right.ToValue(tables);

		var vc = new ValueCollection
		{
			exp.Left.ToValue(tables)
		};

		if (right is FunctionValue lst && lst.Name == "coalesce")
		{
			foreach (var item in lst.Arguments)
			{
				vc.Add(item);
			}
		}
		else
		{
			vc.Add(right);
		}

		return vc;
	}
}
