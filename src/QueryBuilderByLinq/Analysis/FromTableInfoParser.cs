﻿using Carbunql.Clauses;
using Carbunql.Tables;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryBuilderByLinq.Analysis;

public static class FromTableInfoParser
{
	public static FromTableInfo? Parse(Expression exp)
	{
		foreach (var item in exp.GetExpressions().ToList())
		{
			var info = ParseCore(item);
			if (info != null)
			{
				return info;
			}
		}
		return null;
	}

	private static FromTableInfo? ParseCore(Expression exp)
	{
		var from = ParseCore2Arguments(exp);
		if (from != null) return from;
		from = ParseCore3Arguments(exp);
		return from;
	}

	private static FromTableInfo? ParseCore2Arguments(Expression exp)
	{
		if (exp is not MethodCallExpression) return null;

		var method = (MethodCallExpression)exp;
		if (method.Arguments.Count != 2) return null;

		var ce = method.GetArgument<ConstantExpression>(0);
		if (ce == null) return null;

		var operand = method.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>();
		if (operand == null) return null;
		if (operand.Parameters.Count != 1) return null;

		var parameter = operand.Parameters[0];
		if (parameter.Type == typeof(DualTable)) return null;

		if (TryParse(ce, parameter, out var f))
		{
			return f;
		}
		// nest sytax
		return new FromTableInfo(parameter.ToTypeTable(), parameter.Name!);
	}

	/// <summary>
	/// Parsing process when the argument is 3.
	/// Analysis processing when using CTE.
	/// </summary>
	/// <param name="exp"></param>
	/// <returns></returns>
	private static FromTableInfo? ParseCore3Arguments(Expression exp)
	{
		if (exp is not MethodCallExpression) return null;

		var method = (MethodCallExpression)exp;
		if (method.Arguments.Count != 3) return null;

		var ce = method.GetArgument<ConstantExpression>(0);
		var body = method.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>().GetBody<MethodCallExpression>(); ;
		if (body == null) return null;

		if (body.Method.Name == nameof(Sql.FromTable))
		{
			// no relation pattern.
			var operand = method.GetArgument<UnaryExpression>(2).GetOperand<LambdaExpression>();
			if (operand == null) return null;
			if (operand.Parameters.Count != 2) return null;

			var parameter = operand.Parameters[1];
			return ParseCore3ArgumentsFromTable(body, parameter);
		}
		else if (ce != null && (body.Method.Name == nameof(Sql.InnerJoinTable) || body.Method.Name == nameof(Sql.LeftJoinTable) || body.Method.Name == nameof(Sql.CrossJoinTable)))
		{
			// has relation pattern.
			var operand = method.GetArgument<UnaryExpression>(2).GetOperand<LambdaExpression>();
			if (operand == null || operand.Parameters.Count != 2) return null;

			var parameter = operand.Parameters[0];

			if (TryParse(ce, parameter, out var f))
			{
				return f;
			}
			return new FromTableInfo(parameter.ToTypeTable(), parameter.Name!);
		}

		return null;
	}

	private static bool TryParse(ConstantExpression methodBody, ParameterExpression parameter, out FromTableInfo from)
	{
		from = null!;

		if (methodBody.Value is IQueryable q && q.Provider is TableQuery provider)
		{
			if (provider.InnerQuery != null)
			{
				// subquery pattern.
				from = new FromTableInfo(provider.InnerQuery.ToQueryAsPostgres(), parameter.Name!);
				return true;
			}
			else
			{
				// override table name pattern.
				from = new FromTableInfo(provider.TableName, parameter.Name!);
				return true;
			}
		}
		return false;
	}

	private static FromTableInfo? ParseCore3ArgumentsFromTable(MethodCallExpression body, ParameterExpression parameter)
	{
		if (body.Method.Name != nameof(Sql.FromTable)) throw new InvalidProgramException();

		var alias = parameter.Name!;

		//table
		if (body.Arguments.Count == 0)
		{
			// type table pattern.
			// ex.From<T>()
			return new FromTableInfo(parameter.ToTypeTable(), alias);
		}

		if (body.Arguments.Count != 1) return null;

		if (body.Arguments[0] is ParameterExpression)
		{
			// single common table pattern.
			// ex.From(IQueryable)
			var table = body.GetArgument<ParameterExpression>(0)!.Name!;
			return new FromTableInfo(table, alias);
		}
		else if (body.Arguments[0] is MemberExpression)
		{
			var m = (MemberExpression)body.Arguments[0];
			if (m.Expression is ConstantExpression)// && lambda.Parameters.Count == 1
			{
				// subquery pattern.
				return ParseAsSubQuery(m, alias);
			}
			else
			{
				// many common tables pattern.
				// ex.From(IQueryable)
				return new FromTableInfo(m.Member!.Name!, alias);
			}
		}
		else if (body.Arguments[0] is ConstantExpression)
		{
			// string table pattern.
			// ex.From<T>("TableName")
			var table = body.GetArgument<ConstantExpression>(0)?.Value?.ToString()!;
			return new FromTableInfo(table, alias);
		}

		return null;
	}

	private static FromTableInfo? ParseAsSubQuery(MemberExpression m, string alias)
	{
		if (m.Expression is not ConstantExpression) return null;

		var instance = ((ConstantExpression)m.Expression).Value;
		var member = m.Member;

		object? value = null;
		if (member is FieldInfo fieldInfo)
		{
			value = fieldInfo.GetValue(instance);
		}
		else if (member is PropertyInfo propertyInfo)
		{
			value = propertyInfo.GetValue(instance);
		}
		if (value is IQueryable q)
		{
			return new FromTableInfo(q.AsQueryable().ToQueryAsPostgres(), alias);
		}
		return null;
	}

	private static SelectableTable ToTypeTable(this ParameterExpression prm)
	{
		var t = new PhysicalTable()
		{
			Table = prm.Type.ToTableName(),
			ColumnNames = prm.Type.GetProperties().ToList().Select(x => x.Name).ToList()
		};
		return t.ToSelectable();
	}
}
