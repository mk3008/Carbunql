using Carbunql;
using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Extensions;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Postgres.Analysis;

internal static class MethodCallExpressionExpression
{
	internal static InClause ToInClause(this MethodCallExpression exp, List<string> tables)
	{
		if (exp.Method.Name != nameof(Sql.InAs)) throw new InvalidProgramException();

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
		if (exp.Method.Name != nameof(Sql.ExistsAs)) throw new InvalidProgramException();

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

	internal static FunctionValue ToConcatValue(this MethodCallExpression exp, List<string> tables)
	{
		var collection = exp.Arguments.Select(x => x.ToValue(tables)).ToList();
		var args = new ValueCollection(collection);
		return new FunctionValue("concat", args);
	}

	internal static CastValue ToStringValue(this MethodCallExpression exp, List<string> tables)
	{
		var m = (MemberExpression)exp.Object!;
		if (typeof(string).ToTryDbType(out var tp))
		{
			return new CastValue(m.ToValue(tables), "::", tp);
		}
		throw new NotSupportedException();
	}

	internal static FunctionValue ToTrimStartValue(this MethodCallExpression exp, List<string> tables)
	{
		var m = (MemberExpression)exp.Object!;
		return new FunctionValue("ltrim", m.ToValue(tables));
	}

	internal static FunctionValue ToTrimEndValue(this MethodCallExpression exp, List<string> tables)
	{
		var m = (MemberExpression)exp.Object!;
		return new FunctionValue("rtrim", m.ToValue(tables));
	}

	internal static FunctionValue ToTrimValue(this MethodCallExpression exp, List<string> tables)
	{
		var m = (MemberExpression)exp.Object!;
		return new FunctionValue("trim", m.ToValue(tables));
	}

	internal static ValueBase ToAnyFunctionValue(this MethodCallExpression exp, List<string> tables)
	{
		if (exp.Object == null)
		{
			var value = exp.Arguments[1].ToValue(tables);
			var arg = exp.Arguments[0].ToValue(tables);
			if (arg is ParameterValue)
			{
				return value.Equal(new FunctionValue("any", arg));
			}
			else
			{
				var arrayvalue = new ArrayValue(arg);
				return value.Equal(new FunctionValue("any", arrayvalue));
			}
		}
		else
		{
			var value = exp.Arguments.First().ToValue(tables);
			var arg = (MemberExpression)exp.Object!;
			return value.Equal(new FunctionValue("any", arg.ToValue(tables)));
		}
	}

	internal static LikeClause ToContainsLikeClause(this MethodCallExpression exp, List<string> tables)
	{
		var arg = exp.Arguments.First().ToValue(tables);
		var m = (MemberExpression)exp.Object!;
		return CreateLikeClause(m.ToValue(tables), new[] { new LiteralValue("'%'"), arg, new LiteralValue("'%'") });
	}

	internal static LikeClause ToStartsWithLikeClause(this MethodCallExpression exp, List<string> tables)
	{
		var arg = exp.Arguments.First().ToValue(tables);
		var m = (MemberExpression)exp.Object!;
		return CreateLikeClause(m.ToValue(tables), new[] { arg, new LiteralValue("'%'") });
	}

	internal static LikeClause ToEndsWithLikeClause(this MethodCallExpression exp, List<string> tables)
	{
		var arg = exp.Arguments.First().ToValue(tables);
		var m = (MemberExpression)exp.Object!;
		return CreateLikeClause(m.ToValue(tables), new[] { new LiteralValue("'%'"), arg });
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

	internal static FunctionValue ToFunctionValue(this MethodCallExpression exp, List<string> tables)
	{
		PartitionClause? partition = null;
		OrderClause? order = null;
		ValueBase? arg = null;

		for (int i = 0; i < exp.Arguments.Count; i++)
		{
			var argval = exp.Arguments[i].ToValue(tables);
			var name = exp.Method.GetParameters()[i].Name;
			if (name == "partitionby")
			{
				if (argval is ValueCollection vc)
				{
					partition = new PartitionClause(vc.ToList());
				}
				continue;
			}
			else if (name == "orderby")
			{
				if (argval is ValueCollection vc)
				{
					order = new OrderClause(vc.ToList<IQueryCommandable>());
				}
				continue;
			}
			if (partition == null && order == null && arg == null)
			{
				arg = argval;
				continue;
			}
			throw new NotSupportedException();
		}

		var fn = exp.Method.Name.ToDbFunction();
		var v = arg != null ? new FunctionValue(fn, arg) : new FunctionValue(fn);

		if (partition != null && order != null)
		{
			v.Over = new OverClause(new WindowDefinition(partition, order));
		}
		else if (partition != null && order == null)
		{
			v.Over = new OverClause(new WindowDefinition(partition));
		}
		else if (partition == null && order != null)
		{
			v.Over = new OverClause(new WindowDefinition(order));
		}

		return v;
	}

	internal static ValueBase ToCastValue(this MethodCallExpression exp, List<string> tables)
	{
		var v = exp.Arguments[0].ToValue(tables);

		if (exp.Type.ToTryDbType(out var dbType))
		{
			return new CastValue(v, "::", dbType);
		}
		return v;
	}

	private static string ToDbFunction(this string method)
	{
		if (method.IsEqualNoCase(nameof(Sql.RowNumber))) return "row_number";
		return method;
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
}
