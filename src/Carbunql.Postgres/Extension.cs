using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;
using System.Linq.Expressions;
using System.Reflection;

namespace Carbunql.Postgres;

public static class ExpressionExtension
{
	public static (FromClause, T) As<T>(this FromClause source, string alias)
	{
		source.As(alias);
		var r = (T)Activator.CreateInstance(typeof(T))!;
		return (source, r);
	}

	public static SelectableItem Select(this SelectQuery source, Expression<Func<object>> fnc)
	{
		var v = fnc.Body.ToValue();
		var item = new SelectableItem(v, v.GetDefaultName());
		source.SelectClause ??= new();
		source.SelectClause.Add(item);
		return item;
	}

	public static T On<T>(this (Relation relation, T record) source, Expression<Func<T, bool>> predicate)
	{
		var v = predicate.Body.ToValue();

		source.relation.On((_) => v);

		return source.record;
	}

	public static ValueBase Where(this SelectQuery source, Expression<Func<bool>> predicate)
	{
		var v = predicate.Body.ToValue();

		if (v is BracketValue)
		{
			source.Where(v);
		}
		else
		{
			source.Where(new BracketValue(v));
		}

		return v;
	}

	private static ValueBase ToValueExpression(this BinaryExpression exp)
	{
		var op = string.Empty;
		switch (exp.NodeType)
		{
			case ExpressionType.Add:
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

			case ExpressionType.Multiply:
				op = "*";
				break;
			case ExpressionType.Subtract:
				op = "-";
				break;

			case ExpressionType.AddChecked:
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

		if (string.IsNullOrEmpty(op))
		{
			throw new NotSupportedException($"NodeType:{exp.NodeType}");
		}

		var isBracket = exp.ToString().StartsWith("(");

		var left = exp.Left.ToValue();
		var right = exp.Right.ToValue();

		if ((left is LiteralValue lv && lv.IsNullValue) || (right is LiteralValue rv && rv.IsNullValue))
		{
			op = (op == "=") ? "is" : "is not";
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

	private static ValueBase ToValue(this Expression exp)
	{

		if (exp.NodeType == ExpressionType.Constant)
		{
			return ValueParser.Parse(exp.ToString());
		}

		if (exp.NodeType == ExpressionType.MemberAccess)
		{
			return ((MemberExpression)exp).ToValue();
		}

		if (exp is UnaryExpression unary)
		{
			var v = unary.ToValue();
			if (exp.NodeType == ExpressionType.Convert) return v;
			if (exp.NodeType == ExpressionType.Not) return new NegativeValue(v.ToBracket());
			throw new NotSupportedException();
		}

		if (exp is NewExpression ne)
		{
			var args = ne.Arguments.Select(x => x.ToObject()).ToArray();

			var d = ne.Constructor!.Invoke(args);

			if (ne.Type == typeof(DateTime))
			{
				return ((DateTime)d).ToValue();
			}

			return new LiteralValue($"'{d}'");
		}

		return ((BinaryExpression)exp).ToValueExpression();
	}

	private static ValueBase ToValue(this UnaryExpression unary)
	{
		if (unary.Operand is MemberExpression prop)
		{
			var column = prop.Member.Name;

			if (prop.Expression is MemberExpression tp)
			{
				var table = tp.Member.Name;
				return new ColumnValue(table, column);
			}
			else
			{
				throw new NotSupportedException();
			}
		}
		if (unary.Operand is ConstantExpression cons)
		{
			return cons.ExecuteAndConvert();
		}

		return ((BinaryExpression)unary.Operand).ToValueExpression();
	}

	private static ValueBase ToValue(this MemberExpression exp)
	{
		if (exp.ToString() == "DateTime.Now")
		{
			return ValueParser.Parse("current_timestamp");
		}

		if (exp.Expression is null && exp.Member is PropertyInfo prop)
		{
			if (prop.GetGetMethod()!.IsStatic)
			{
				var d = prop!.GetValue(null);
				if (exp.Type == typeof(DateTime) && d != null)
				{
					return ((DateTime)d).ToValue();
				}
			}
		}

		if (exp.Expression is null)
		{
			throw new NotSupportedException($"Expression is null.");
		}

		if (exp.Expression is ParameterExpression prm)
		{
			var table = prm.Name!;
			var column = exp.Member.Name;
			return new ColumnValue(table, column);
		}

		if (exp.Expression is MemberExpression mem)
		{
			//If there is a RecordDefinition attribute,
			//treat it as an expression without compiling it.
			if (mem.Type.GetCustomAttribute<RecordDefinitionAttribute>() != null)
			{
				var table = mem.Member.Name;
				var column = exp.Member.Name;
				return new ColumnValue(table, column);
			}

			return exp.ExecuteAndConvert();
		}

		if (exp.Expression is ConstantExpression)
		{
			return exp.ExecuteAndConvert();
		}

		throw new NotSupportedException($"propExpression.Expression type:{exp.Expression.GetType().Name}");
	}

	private static ValueBase ExecuteAndConvert(this Expression exp)
	{
		if (exp.Type == typeof(DateTime))
		{
			var lm = Expression.Lambda<Func<DateTime>>(exp);
			var d = lm.Compile().Invoke();
			return d.ToValue();
		}
		if (exp.Type == typeof(string))
		{
			var lm = Expression.Lambda<Func<string>>(exp);
			var d = lm.Compile().Invoke();
			return ValueParser.Parse($"'{d}'");
		}
		if (exp.Type == typeof(int))
		{
			var lm = Expression.Lambda<Func<int>>(exp);
			var d = lm.Compile().Invoke();
			return ValueParser.Parse(d.ToString());
		}
		if (exp.Type == typeof(double))
		{
			var lm = Expression.Lambda<Func<double>>(exp);
			var d = lm.Compile().Invoke();
			return ValueParser.Parse(d.ToString());
		}
		if (exp.Type == typeof(float))
		{
			var lm = Expression.Lambda<Func<float>>(exp);
			var d = lm.Compile().Invoke();
			return ValueParser.Parse(d.ToString());
		}
		if (exp.Type == typeof(bool))
		{
			var lm = Expression.Lambda<Func<bool>>(exp);
			var d = lm.Compile().Invoke();
			return ValueParser.Parse(d.ToString());
		}
		return ValueParser.Parse(exp.ToString());
	}

	private static ValueBase ToValue(this DateTime d)
	{
		return new FunctionValue("cast", new CastValue(new LiteralValue($"'{d}'"), "as", new LiteralValue("timestamp")));
	}

	private static object ToObject(this Expression exp)
	{
		if (exp.NodeType != ExpressionType.Constant) { throw new NotSupportedException(); }

		if (exp.Type == typeof(string)) return exp.ToString();
		if (exp.Type == typeof(int)) return int.Parse(exp.ToString());

		throw new NotSupportedException();
	}
}
