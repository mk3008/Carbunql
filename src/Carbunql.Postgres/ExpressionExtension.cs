using Carbunql.Analysis.Parser;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Carbunql.Postgres;

public static class ExpressionExtension
{
	internal static ValueBase ToValueExpression(this BinaryExpression exp, List<string> tables)
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

			case ExpressionType.Coalesce:
				return exp.ToCoalesceValue(tables);

			case ExpressionType.AddChecked:
				break;
			case ExpressionType.ArrayLength:
				break;
			case ExpressionType.ArrayIndex:
				break;
			case ExpressionType.Call:
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

		var left = exp.Left.ToValue(tables);
		var right = exp.Right.ToValue(tables);

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
				if (mc.Method.Name == "ExistsAs") return mc.ToExistsExpression(tables);
				if (mc.Method.Name == "InAs") return mc.ToInClause(tables);
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

			return ((MethodCallExpression)exp).ToParameterValue();
		}

		if (exp is NewArrayExpression arrayexp)
		{
			return arrayexp.ToValue(tables);
		}

		return ((BinaryExpression)exp).ToValueExpression(tables);
	}

	internal static List<(ColumnValue Argument, ColumnValue Column)> ToInClause(ColumnValue predicate, string alias)
	{
		var lst = new List<(ColumnValue Argument, ColumnValue Column)>();

		if (predicate.TableAlias == alias)
		{
			var op = predicate.OperatableValue;
			if (op == null || op.Operator != "=") throw new InvalidProgramException();
			var body = op.Value as ColumnValue;
			if (body == null) throw new NullReferenceException(nameof(body));

			var column = new ColumnValue(predicate.TableAlias, predicate.Column);
			var arg = new ColumnValue(body.TableAlias, body.Column);

			lst.Add((arg, column));

			if (op.Value.OperatableValue != null) lst.AddRange(ToInClause((ColumnValue)op.Value.OperatableValue.Value, alias));
		}
		else
		{
			var op = predicate.OperatableValue;
			if (op == null || op.Operator != "=") throw new InvalidProgramException();
			var body = op.Value as ColumnValue;
			if (body!.TableAlias != alias) throw new InvalidProgramException();

			var arg = new ColumnValue(predicate.TableAlias, predicate.Column);
			var column = new ColumnValue(body.TableAlias, body.Column);

			lst.Add((arg, column));

			if (op.Value.OperatableValue != null) lst.AddRange(ToInClause((ColumnValue)op.Value.OperatableValue.Value, alias));
		}

		return lst;
	}

	internal static FunctionValue ToCoalesceValue(this BinaryExpression exp, List<string> tables)
	{
		var vc = exp.ToValueCollection(tables);

		return new FunctionValue("coalesce", vc);
	}

	internal static ValueCollection ToValueCollection(this BinaryExpression exp, List<string> tables)
	{
		var right = exp.Right.ToValue(tables);

		var vc = new ValueCollection
		{
			exp.Left.ToValue(tables)
		};

		if (right is FunctionValue lst && lst.Name == "coalesce")
		{
			foreach (var item in lst.Argument)
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

	internal static ValueCollection ToValue(this NewArrayExpression exp, List<string> tables)
	{
		var vc = new ValueCollection();
		foreach (var item in exp.Expressions)
		{
			vc.Add(item.ToValue(tables));
		}
		return vc;
	}

	internal static FunctionValue ToFunctionValue(this MethodCallExpression exp, List<string> tables)
	{
		var lexp = exp.Arguments[1].Execute() as LambdaExpression;
		if (lexp == null) throw new InvalidProgramException();

		var arg = lexp.Body.ToValue(tables);

		//throw new Exception();
		//var av = arg.B
		var v = new FunctionValue(exp.Method.Name, arg);
		return v;
	}

	internal static InClause ToInClause(this LambdaExpression predicate, Action<SelectQuery> fromBuilder, string alias, List<string> tables)
	{
		var pv = predicate.Body.ToValue(tables);

		var sq = new SelectQuery();
		fromBuilder(sq);

		var cv = (pv is ColumnValue v) ? v : (pv is BracketValue bv && bv.Inner is ColumnValue bcv) ? bcv : throw new NotSupportedException();

		var lst = ToInClause(cv, alias);

		var args = new ValueCollection();
		foreach (var item in lst)
		{
			args.Add(item.Argument);
			sq.Select(item.Column).As(item.Column.Column);
		}
		ValueBase arg = (args.Count == 1) ? args : args.ToBracket();
		return new InClause(arg, sq.ToValue());
	}

	internal static InClause ToInClause(this MethodCallExpression exp, List<string> tables)
	{
		if (exp.Method.Name != "InAs") throw new InvalidProgramException();

		if (exp.Arguments.Count < 2) throw new NotSupportedException();

		if (exp.Arguments.Count == 2)
		{
			var tableType = exp.Method.GetGenericArguments()[0];
			var predicate = exp.Arguments[1].Execute() as LambdaExpression;
			if (tableType == null || predicate == null) throw new NullReferenceException();

			var alias = predicate.Parameters[0].Name;
			if (string.IsNullOrEmpty(alias)) throw new InvalidProgramException();

			return predicate.ToInClause(sq => sq.From(tableType.ToTableName()).As(alias), alias, tables);
		}

		var arg1 = exp.Arguments[1].Execute();

		if (exp.Arguments.Count == 3 && arg1 is IReadQuery query)
		{
			var tableType = exp.Method.GetGenericArguments()[0];
			var predicate = exp.Arguments[2].Execute() as LambdaExpression;
			if (tableType == null || query == null || predicate == null) throw new NullReferenceException();

			var alias = predicate.Parameters[0].Name;
			if (string.IsNullOrEmpty(alias)) throw new InvalidProgramException();

			return predicate.ToInClause(sq => sq.From(query).As(alias), alias, tables);
		}

		if (exp.Arguments.Count == 3 && arg1 is string table)
		{
			var tableType = exp.Method.GetGenericArguments()[0];
			var predicate = exp.Arguments[2].Execute() as LambdaExpression;
			if (tableType == null || table == null || predicate == null) throw new NullReferenceException();

			var alias = predicate.Parameters[0].Name;
			if (string.IsNullOrEmpty(alias)) throw new InvalidProgramException();

			return predicate.ToInClause(sq => sq.From(table).As(alias), alias, tables);
		}

		throw new NotSupportedException();
	}

	internal static ExistsExpression ToExistsExpression(this MethodCallExpression exp, List<string> tables)
	{
		if (exp.Method.Name != "ExistsAs") throw new InvalidProgramException();

		if (exp.Arguments.Count < 2) throw new NotSupportedException();

		if (exp.Arguments.Count == 2)
		{
			var tableType = exp.Method.GetGenericArguments()[0];
			var predicate = exp.Arguments[1].Execute() as LambdaExpression;
			if (tableType == null || predicate == null) throw new NullReferenceException();

			var alias = predicate.Parameters[0].Name;
			if (string.IsNullOrEmpty(alias)) throw new InvalidProgramException();

			return predicate.ToExistsExpression(sq => sq.From(tableType.ToTableName()).As(alias), alias, tables);
		}

		var arg1 = exp.Arguments[1].Execute();

		if (exp.Arguments.Count == 3 && arg1 is IReadQuery query)
		{
			var tableType = exp.Method.GetGenericArguments()[0];
			var predicate = exp.Arguments[2].Execute() as LambdaExpression;
			if (tableType == null || query == null || predicate == null) throw new NullReferenceException();

			var alias = predicate.Parameters[0].Name;
			if (string.IsNullOrEmpty(alias)) throw new InvalidProgramException();

			return predicate.ToExistsExpression(sq => sq.From(query).As(alias), alias, tables);
		}

		if (exp.Arguments.Count == 3 && arg1 is string table)
		{
			var tableType = exp.Method.GetGenericArguments()[0];
			var predicate = exp.Arguments[2].Execute() as LambdaExpression;
			if (tableType == null || table == null || predicate == null) throw new NullReferenceException();

			var alias = predicate.Parameters[0].Name;
			if (string.IsNullOrEmpty(alias)) throw new InvalidProgramException();

			return predicate.ToExistsExpression(sq => sq.From(table).As(alias), alias, tables);
		}

		throw new NotSupportedException();
	}

	internal static ExistsExpression ToExistsExpression(this LambdaExpression predicate, Action<SelectQuery> fromBuilder, string alias, List<string> tables)
	{
		var lst = new List<string>();
		lst.AddRange(tables);
		lst.Add(alias);
		var condition = predicate.Body.ToValue(lst);

		var sq = new SelectQuery();
		fromBuilder(sq);
		sq.SelectAll();

		sq.Where(condition);

		return new ExistsExpression(sq);
	}

	internal static FunctionValue ToConcatValue(this MethodCallExpression exp, List<string> tables)
	{
		if (exp.Method.Name != "Concat") throw new InvalidProgramException();

		var collection = exp.Arguments.Select(x => x.ToValue(tables)).ToList();
		var args = new ValueCollection(collection);
		return new FunctionValue("concat", args);
	}

	internal static FunctionValue ToTrimStartValue(this MethodCallExpression exp, List<string> tables)
	{
		if (exp.Method.Name != "TrimStart") throw new InvalidProgramException();

		var m = (MemberExpression)exp.Object!;
		return new FunctionValue("ltrim", m.ToValue(tables));
	}

	internal static FunctionValue ToTrimEndValue(this MethodCallExpression exp, List<string> tables)
	{
		if (exp.Method.Name != "TrimEnd") throw new InvalidProgramException();

		var m = (MemberExpression)exp.Object!;
		return new FunctionValue("rtrim", m.ToValue(tables));
	}

	internal static FunctionValue ToTrimValue(this MethodCallExpression exp, List<string> tables)
	{
		if (exp.Method.Name != "Trim") throw new InvalidProgramException();

		var m = (MemberExpression)exp.Object!;
		return new FunctionValue("trim", m.ToValue(tables));
	}

	internal static ValueBase ToAnyFunctionValue(this MethodCallExpression exp, List<string> tables)
	{
		if (exp.Method.Name != "Contains") throw new InvalidProgramException();

		if (exp.Object == null)
		{
			var value = exp.Arguments[1].ToValue(tables);
			var arg = exp.Arguments[0].ToValue(tables);
			return value.Equal(new FunctionValue("any", arg));
		}
		else
		{
			var value = exp.Arguments.First().ToValue(tables);
			var arg = (MemberExpression)exp.Object!;
			return value.Equal(new FunctionValue("any", arg.ToValue(tables)));
		}
	}

	internal static LikeClause CreateLikeClause(ValueBase value, params ValueBase[] args)
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

	internal static LikeClause ToContainsLikeClause(this MethodCallExpression exp, List<string> tables)
	{
		if (exp.Method.Name != "Contains") throw new InvalidProgramException();

		var arg = exp.Arguments.First().ToValue(tables);
		var m = (MemberExpression)exp.Object!;
		return CreateLikeClause(m.ToValue(tables), new[] { new LiteralValue("'%'"), arg, new LiteralValue("'%'") });
	}

	internal static LikeClause ToStartsWithLikeClause(this MethodCallExpression exp, List<string> tables)
	{
		if (exp.Method.Name != "StartsWith") throw new InvalidProgramException();

		var arg = exp.Arguments.First().ToValue(tables);
		var m = (MemberExpression)exp.Object!;
		return CreateLikeClause(m.ToValue(tables), new[] { arg, new LiteralValue("'%'") });
	}

	internal static LikeClause ToEndsWithLikeClause(this MethodCallExpression exp, List<string> tables)
	{
		if (exp.Method.Name != "EndsWith") throw new InvalidProgramException();

		var arg = exp.Arguments.First().ToValue(tables);
		var m = (MemberExpression)exp.Object!;
		return CreateLikeClause(m.ToValue(tables), new[] { new LiteralValue("'%'"), arg });
	}

	internal static ValueBase ToValue(this NewExpression exp, List<string> tables)
	{
		var args = exp.Arguments.Select(x => x.ToObject()).ToArray();

		var d = exp.Constructor!.Invoke(args);

		if (exp.Type == typeof(DateTime))
		{
			return ((DateTime)d).ToValue();
		}

		return ValueParser.Parse($"'{d}'");
	}

	internal static ParameterValue ToParameterValue(this MethodCallExpression exp)
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

	internal static ParameterValue ToParameterValue(this InvocationExpression exp)
	{
		var value = exp.Execute();

		var inner = exp.Expression;
		var key = ((MemberExpression)inner).Member.Name.ToParameterName("invoke");

		var prm = new ParameterValue(key, value);
		return prm;
	}

	internal static ValueBase ToValue(this UnaryExpression exp, List<string> tables)
	{
		var v = exp.ToValueCore(tables);
		if (exp.NodeType == ExpressionType.Convert) return v;
		if (exp.NodeType == ExpressionType.Not)
		{
			if (v is ExistsExpression) return new NegativeValue(v);
			if (v is InClause ic)
			{
				ic.IsNegative = true;
				return ic;
			}
			if (v is LikeClause lc)
			{
				lc.IsNegative = true;
				return lc;
			}
			return new NegativeValue(v.ToBracket());
		}
		throw new NotSupportedException();
	}

	internal static ValueBase ToValueCore(this UnaryExpression exp, List<string> tables)
	{
		if (exp.Operand is MemberExpression mem)
		{
			return mem.ToValue(tables);
		}

		if (exp.Operand is ConstantExpression cons)
		{
			return cons.ToValue();
		}

		if (exp.Operand is MethodCallExpression ce)
		{
			return ce.ToValue(tables);
		}

		return ((BinaryExpression)exp.Operand).ToValueExpression(tables);
	}

	internal static ValueBase ToValue(this MemberExpression exp, List<string> tables)
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
			var table = tables.Where(x => x == mem.Member.Name).FirstOrDefault();

			if (!string.IsNullOrEmpty(table))
			{
				var column = exp.Member.Name;
				return new ColumnValue(table, column);
			}

			return exp.ToValue();
		}

		if (exp.Expression is ConstantExpression)
		{
			return exp.ToValue();
		}

		throw new NotSupportedException($"propExpression.Expression type:{exp.Expression.GetType().Name}");
	}

	internal static ValueBase ToValue(this ConstantExpression exp)
	{
		var value = exp.Execute();
		if (value == null) return ValueParser.Parse("null");
		if (value is DateTime d) return d.ToValue();
		if (value is string s) return ValueParser.Parse($"'{value}'");
		return new LiteralValue(value.ToString());
	}

	internal static ValueBase ToValue(this MemberExpression exp)
	{
		var value = exp.Execute();

		if (value is IEnumerable<ValueBase> values)
		{
			var vc = new ValueCollection();
			foreach (var item in values)
			{
				vc.Add(item);
			}
			return vc;
		}

		return exp.ToParameterValue(value);
	}

	internal static ValueBase ToParameterValue(this MemberExpression exp, object? value)
	{
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

	internal static object? Execute(this Expression exp)
	{
		var method = typeof(ExpressionExtension)
			.GetMethod(nameof(ExecuteCore), BindingFlags.NonPublic | BindingFlags.Static)!
			.MakeGenericMethod(exp.Type);

		return method.Invoke(null, new object[] { exp });
	}

	internal static T ExecuteCore<T>(this Expression exp)
	{
		var lm = Expression.Lambda<Func<T>>(exp);
		return lm.Compile().Invoke();
	}

	internal static ValueBase ToValue(this DateTime d)
	{
		return new FunctionValue("cast", new CastValue(new LiteralValue($"'{d}'"), "as", new LiteralValue("timestamp")));
	}

	internal static object ToObject(this Expression exp)
	{
		if (exp.NodeType != ExpressionType.Constant) { throw new NotSupportedException(); }

		if (exp.Type == typeof(string)) return exp.ToString();
		if (exp.Type == typeof(int)) return int.Parse(exp.ToString());

		throw new NotSupportedException();
	}
}
