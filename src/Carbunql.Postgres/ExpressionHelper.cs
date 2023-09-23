using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Carbunql.Postgres;

public static class ExpressionHelper
{
	public static void SelectAll(this SelectQuery source, Expression<Func<object>> fnc)
	{
		var v = fnc.Compile().Invoke();
		var exp = (UnaryExpression)fnc.Body;
		var op = (MemberExpression)exp.Operand;

		foreach (var prop in v.GetType().GetProperties())
		{
			source.Select(op.Member.Name, prop.Name);
		}
	}

	public static SelectableItem Select(this SelectQuery source, Expression<Func<object>> fnc)
	{
		var v = fnc.Body.ToValue();
		var item = new SelectableItem(v, v.GetDefaultName());
		source.SelectClause ??= new();
		source.SelectClause.Add(item);
		return item;
	}

	public static (FromClause, T) FromAs<T>(this SelectQuery source, string alias)
	{
		var atr = typeof(T).GetCustomAttribute(typeof(RecordDefinitionAttribute)) as RecordDefinitionAttribute;
		if (atr == null || string.IsNullOrEmpty(atr.Table))
		{
			return source.FromAs<T>(typeof(T).Name, alias);
		}
		else
		{
			return source.FromAs<T>(atr.Table, alias);
		}
	}

	public static (FromClause, T) FromAs<T>(this SelectQuery source, string table, string alias)
	{
		var r = (T)Activator.CreateInstance(typeof(T))!;
		var (from, _) = source.From(table).As(alias);
		return (from, r);
	}

	public static (FromClause, T) As<T>(this FromClause source, string alias)
	{
		source.As(alias);
		var r = (T)Activator.CreateInstance(typeof(T))!;
		return (source, r);
	}

	public static (Relation, T) InnerJoinAs<T>(this FromClause source, string alias)
	{
		var atr = typeof(T).GetCustomAttribute(typeof(RecordDefinitionAttribute)) as RecordDefinitionAttribute;
		if (atr == null || string.IsNullOrEmpty(atr.Table))
		{
			return source.InnerJoinAs<T>(typeof(T).Name, alias);
		}
		else
		{
			return source.InnerJoinAs<T>(atr.Table, alias);
		}
	}

	public static (Relation, T) InnerJoinAs<T>(this FromClause source, string table, string alias)
	{
		return source.InnerJoin(table).As<T>(alias);
	}

	public static (Relation, T) LeftJoinAs<T>(this FromClause source, string alias)
	{
		var atr = typeof(T).GetCustomAttribute(typeof(RecordDefinitionAttribute)) as RecordDefinitionAttribute;
		if (atr == null || string.IsNullOrEmpty(atr.Table))
		{
			return source.LeftJoinAs<T>(typeof(T).Name, alias);
		}
		else
		{
			return source.LeftJoinAs<T>(atr.Table, alias);
		}
	}

	public static (Relation, T) LeftJoinAs<T>(this FromClause source, string table, string alias)
	{
		return source.LeftJoin(table).As<T>(alias);
	}

	public static (Relation, T) RightJoinAs<T>(this FromClause source, string alias)
	{
		var atr = typeof(T).GetCustomAttribute(typeof(RecordDefinitionAttribute)) as RecordDefinitionAttribute;
		if (atr == null || string.IsNullOrEmpty(atr.Table))
		{
			return source.RightJoinAs<T>(typeof(T).Name, alias);
		}
		else
		{
			return source.RightJoinAs<T>(atr.Table, alias);
		}
	}

	public static (Relation, T) RightJoinAs<T>(this FromClause source, string table, string alias)
	{
		return source.RightJoin(table).As<T>(alias);
	}

	public static (Relation, T) CrossJoinAs<T>(this FromClause source, string alias)
	{
		var atr = typeof(T).GetCustomAttribute(typeof(RecordDefinitionAttribute)) as RecordDefinitionAttribute;
		if (atr == null || string.IsNullOrEmpty(atr.Table))
		{
			return source.CrossJoinAs<T>(typeof(T).Name, alias);
		}
		else
		{
			return source.CrossJoinAs<T>(atr.Table, alias);
		}
	}

	public static (Relation, T) CrossJoinAs<T>(this FromClause source, string table, string alias)
	{
		return source.CrossJoin(table).As<T>(alias);
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

	private static ValueBase ToValue(this Expression exp)
	{

		if (exp.NodeType == ExpressionType.Constant)
		{
			return ((ConstantExpression)exp).ToValue();
		}

		if (exp.NodeType == ExpressionType.MemberAccess)
		{
			return ((MemberExpression)exp).ToValue();
		}

		if (exp is UnaryExpression unary)
		{
			return unary.ToValue();
		}

		if (exp is NewExpression ne)
		{
			return ne.ToValue();
		}

		if (exp.NodeType == ExpressionType.Invoke)
		{
			return ((InvocationExpression)exp).ToParameterValue();
		}

		if (exp.NodeType == ExpressionType.Call)
		{
			var mc = (MethodCallExpression)exp;

			if (mc.Method.Name == "Concat") return mc.ToConcatValue();

			if (mc.Method.Name == "Contains")
			{
				if (mc.Object == null && mc.Method.DeclaringType == typeof(Enumerable))
				{
					return mc.ToAnyFunctionValue();
				}
				if (mc.Object != null && typeof(IList).IsAssignableFrom(mc.Object.Type))
				{
					return mc.ToAnyFunctionValue();
				}
				else
				{
					return mc.ToContainsLikeClause();
				}
			}

			if (mc.Object == null) throw new NullReferenceException("MethodCallExpression.Object is null.");
			if (mc.Object.NodeType == ExpressionType.Constant)
			{
				return ((MethodCallExpression)exp).ToParameterValue();
			}


			if (mc.Method.Name == "StartsWith") return mc.ToStartsWithLikeClause();
			if (mc.Method.Name == "EndsWith") return mc.ToEndsWithLikeClause();
			if (mc.Method.Name == "Trim") return mc.ToTrimValue();
			if (mc.Method.Name == "TrimStart") return mc.ToTrimStartValue();
			if (mc.Method.Name == "TrimEnd") return mc.ToTrimEndValue();

			return ((MethodCallExpression)exp).ToParameterValue();
		}

		return ((BinaryExpression)exp).ToValueExpression();
	}

	private static FunctionValue ToConcatValue(this MethodCallExpression exp)
	{
		if (exp.Method.Name != "Concat") throw new InvalidProgramException();

		var collection = exp.Arguments.Select(x => x.ToValue()).ToList();
		var args = new ValueCollection(collection);
		return new FunctionValue("concat", args);
	}

	private static FunctionValue ToTrimStartValue(this MethodCallExpression exp)
	{
		if (exp.Method.Name != "TrimStart") throw new InvalidProgramException();

		var m = (MemberExpression)exp.Object!;
		return new FunctionValue("ltrim", m.ToValue());
	}

	private static FunctionValue ToTrimEndValue(this MethodCallExpression exp)
	{
		if (exp.Method.Name != "TrimEnd") throw new InvalidProgramException();

		var m = (MemberExpression)exp.Object!;
		return new FunctionValue("rtrim", m.ToValue());
	}

	private static FunctionValue ToTrimValue(this MethodCallExpression exp)
	{
		if (exp.Method.Name != "Trim") throw new InvalidProgramException();

		var m = (MemberExpression)exp.Object!;
		return new FunctionValue("trim", m.ToValue());
	}

	private static ValueBase ToAnyFunctionValue(this MethodCallExpression exp)
	{
		if (exp.Method.Name != "Contains") throw new InvalidProgramException();

		if (exp.Object == null)
		{
			var value = exp.Arguments[1].ToValue();
			var arg = exp.Arguments[0].ToValue();
			return value.Equal(new FunctionValue("any", arg));
		}
		else
		{
			var value = exp.Arguments.First().ToValue();
			var arg = (MemberExpression)exp.Object!;
			return value.Equal(new FunctionValue("any", arg.ToValue()));
		}
	}

	private static LikeClause CreateLikeClause(ValueBase value, params ValueBase[] args)
	{
		ValueBase? prm = null;
		foreach (var item in args)
		{
			if (prm == null)
			{
				prm = item;
			}
			else
			{
				prm.AddOperatableValue("||", item);
			}
		}
		if (prm == null) throw new InvalidProgramException();

		return new LikeClause(value, prm);
	}

	private static LikeClause ToContainsLikeClause(this MethodCallExpression exp)
	{
		if (exp.Method.Name != "Contains") throw new InvalidProgramException();

		var arg = exp.Arguments.First().ToValue();
		var m = (MemberExpression)exp.Object!;
		return CreateLikeClause(m.ToValue(), new[] { new LiteralValue("'%'"), arg, new LiteralValue("'%'") });
	}

	private static LikeClause ToStartsWithLikeClause(this MethodCallExpression exp)
	{
		if (exp.Method.Name != "StartsWith") throw new InvalidProgramException();

		var arg = exp.Arguments.First().ToValue();
		var m = (MemberExpression)exp.Object!;
		return CreateLikeClause(m.ToValue(), new[] { arg, new LiteralValue("'%'") });
	}

	private static LikeClause ToEndsWithLikeClause(this MethodCallExpression exp)
	{
		if (exp.Method.Name != "EndsWith") throw new InvalidProgramException();

		var arg = exp.Arguments.First().ToValue();
		var m = (MemberExpression)exp.Object!;
		return CreateLikeClause(m.ToValue(), new[] { new LiteralValue("'%'"), arg });
	}

	private static ValueBase ToValue(this NewExpression exp)
	{
		var args = exp.Arguments.Select(x => x.ToObject()).ToArray();

		var d = exp.Constructor!.Invoke(args);

		if (exp.Type == typeof(DateTime))
		{
			return ((DateTime)d).ToValue();
		}

		return ValueParser.Parse($"'{d}'");
	}

	private static ParameterValue ToParameterValue(this MethodCallExpression exp)
	{
		var value = exp.Execute();

		var key = string.Empty;
		if (exp.Object is MemberExpression mem)
		{
			key = mem.Member.Name + '_' + exp.Method.Name;
		}
		else
		{
			key = exp.Method.Name;
		}
		key = key.ToParameterName("method");

		var prm = new ParameterValue(key, value);
		return prm;
	}

	private static ParameterValue ToParameterValue(this InvocationExpression exp)
	{
		var value = exp.Execute();

		var inner = exp.Expression;
		var key = ((MemberExpression)inner).Member.Name.ToParameterName("invoke");

		var prm = new ParameterValue(key, value);
		return prm;
	}

	private static ValueBase ToValue(this UnaryExpression exp)
	{
		var v = exp.ToValueCore();
		if (exp.NodeType == ExpressionType.Convert) return v;
		if (exp.NodeType == ExpressionType.Not) return new NegativeValue(v.ToBracket());
		throw new NotSupportedException();
	}

	private static ValueBase ToValueCore(this UnaryExpression exp)
	{
		if (exp.Operand is MemberExpression mem)
		{
			return mem.ToValue();
		}

		if (exp.Operand is ConstantExpression cons)
		{
			return cons.ToValue();
		}

		if (exp.Operand is MethodCallExpression ce)
		{
			return ce.ToValue();
		}

		return ((BinaryExpression)exp.Operand).ToValueExpression();
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

			return exp.ToParameterValue();
		}

		if (exp.Expression is ConstantExpression)
		{
			return exp.ToParameterValue();
		}

		throw new NotSupportedException($"propExpression.Expression type:{exp.Expression.GetType().Name}");
	}

	private static ValueBase ToValue(this ConstantExpression exp)
	{
		var value = exp.Execute();
		if (value == null) return ValueParser.Parse("null");
		if (value is DateTime d) return d.ToValue();
		if (value is string s) return ValueParser.Parse($"'{value}'");
		return new LiteralValue(value.ToString());
	}

	private static ParameterValue ToParameterValue(this MemberExpression exp)
	{
		var value = exp.Execute();
		var key = string.Empty;

		if (exp.Expression is MemberExpression m)
		{
			key = m.Member.Name + '_' + exp.Member.Name;
		}
		else
		{
			key = exp.Member.Name;
		}
		key = key.ToParameterName("member");

		var prm = new ParameterValue(key, value);
		return prm;
	}

	private static object? Execute(this Expression exp)
	{
		var method = typeof(ExpressionHelper)
			.GetMethod(nameof(ExecuteCore), BindingFlags.NonPublic | BindingFlags.Static)!
			.MakeGenericMethod(exp.Type);

		return method.Invoke(null, new object[] { exp });
	}

	private static T ExecuteCore<T>(this Expression exp)
	{
		var lm = Expression.Lambda<Func<T>>(exp);
		return lm.Compile().Invoke();
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

	private static string ToSnakeCase(this string input)
	{
		var cleanedInput = Regex.Replace(input, @"[^a-zA-Z0-9]", "");
		if (string.IsNullOrEmpty(cleanedInput)) return string.Empty;
		return Regex.Replace(cleanedInput, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
	}

	private static string ToParameterName(this string input, string prefix)
	{
		var name = input.ToSnakeCase();
		if (string.IsNullOrEmpty(name)) throw new Exception("key name is empty.");

		if (!string.IsNullOrEmpty(prefix))
		{
			name = prefix.ToSnakeCase() + "_" + name;
		}

		name = ':' + name;
		if (name.Length > 60)
		{
			name = name.Substring(0, 60);
		}
		return name;
	}
}
