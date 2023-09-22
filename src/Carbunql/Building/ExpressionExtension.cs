using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Building;

public static class ExpressionExtension
{
	public static ValueBase ToValueExpression(this BinaryExpression exp)
	{
		var op = string.Empty;
		switch (exp.NodeType)
		{
			case ExpressionType.Add:
			case ExpressionType.AddChecked:
				op = "+";
				break;
			case ExpressionType.And:
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
			case ExpressionType.Or:
			case ExpressionType.OrElse:
				op = "or";
				break;
			case ExpressionType.NotEqual:
				op = "<>";
				break;

			case ExpressionType.ArrayLength:
				break;
			case ExpressionType.ArrayIndex:
				break;
			case ExpressionType.Call:
				break;
			case ExpressionType.Coalesce:
				break;
			case ExpressionType.Conditional:
				break;
			case ExpressionType.Constant:
				break;
			case ExpressionType.Convert:
				break;
			case ExpressionType.ConvertChecked:
				break;


			case ExpressionType.ExclusiveOr:
				break;

			case ExpressionType.Invoke:
				break;
			case ExpressionType.Lambda:
				break;
			case ExpressionType.LeftShift:
				break;

			case ExpressionType.ListInit:
				break;
			case ExpressionType.MemberAccess:
				break;
			case ExpressionType.MemberInit:
				break;
			case ExpressionType.Modulo:
				break;
			case ExpressionType.Multiply:
				break;
			case ExpressionType.MultiplyChecked:
				break;
			case ExpressionType.Negate:
				break;
			case ExpressionType.UnaryPlus:
				break;
			case ExpressionType.NegateChecked:
				break;
			case ExpressionType.New:
				break;
			case ExpressionType.NewArrayInit:
				break;
			case ExpressionType.NewArrayBounds:
				break;
			case ExpressionType.Not:
				break;
			case ExpressionType.Parameter:
				break;
			case ExpressionType.Power:
				break;
			case ExpressionType.Quote:
				break;
			case ExpressionType.RightShift:
				break;
			case ExpressionType.Subtract:
				break;
			case ExpressionType.SubtractChecked:
				break;
			case ExpressionType.TypeAs:
				break;
			case ExpressionType.TypeIs:
				break;
			case ExpressionType.Assign:
				break;
			case ExpressionType.Block:
				break;
			case ExpressionType.DebugInfo:
				break;
			case ExpressionType.Decrement:
				break;
			case ExpressionType.Dynamic:
				break;
			case ExpressionType.Default:
				break;
			case ExpressionType.Extension:
				break;
			case ExpressionType.Goto:
				break;
			case ExpressionType.Increment:
				break;
			case ExpressionType.Index:
				break;
			case ExpressionType.Label:
				break;
			case ExpressionType.RuntimeVariables:
				break;
			case ExpressionType.Loop:
				break;
			case ExpressionType.Switch:
				break;
			case ExpressionType.Throw:
				break;
			case ExpressionType.Try:
				break;
			case ExpressionType.Unbox:
				break;
			case ExpressionType.AddAssign:
				break;
			case ExpressionType.AndAssign:
				break;
			case ExpressionType.DivideAssign:
				break;
			case ExpressionType.ExclusiveOrAssign:
				break;
			case ExpressionType.LeftShiftAssign:
				break;
			case ExpressionType.ModuloAssign:
				break;
			case ExpressionType.MultiplyAssign:
				break;
			case ExpressionType.OrAssign:
				break;
			case ExpressionType.PowerAssign:
				break;
			case ExpressionType.RightShiftAssign:
				break;
			case ExpressionType.SubtractAssign:
				break;
			case ExpressionType.AddAssignChecked:
				break;
			case ExpressionType.MultiplyAssignChecked:
				break;
			case ExpressionType.SubtractAssignChecked:
				break;
			case ExpressionType.PreIncrementAssign:
				break;
			case ExpressionType.PreDecrementAssign:
				break;
			case ExpressionType.PostIncrementAssign:
				break;
			case ExpressionType.PostDecrementAssign:
				break;
			case ExpressionType.TypeEqual:
				break;
			case ExpressionType.OnesComplement:
				break;
			case ExpressionType.IsTrue:
				break;
			case ExpressionType.IsFalse:
				break;
			default:
				break;
		}

		if (string.IsNullOrEmpty(op)) throw new Exception();

		var isBracket = exp.ToString().StartsWith("(");

		var left = exp.Left.ToValue();
		var right = exp.Right.ToValue();

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

	public static ValueBase ToValue(this Expression exp)
	{
		if (exp.NodeType == ExpressionType.Constant)
		{
			return ValueParser.Parse(exp.ToString());
		}
		if (exp.NodeType == ExpressionType.MemberAccess)
		{
			var propExpression = (MemberExpression)exp;
			var column = propExpression.Member.Name;

			if (propExpression.Expression is ParameterExpression prm)
			{
				var table = prm.Name!;
				return new ColumnValue(table, column);
			}
			else if (propExpression.Expression is MemberExpression mem)
			{
				var table = mem.Member.Name;
				return new ColumnValue(table, column);
			}
			else
			{
				throw new NotSupportedException();
			}
		}
		return ((BinaryExpression)exp).ToValueExpression();
	}
}
