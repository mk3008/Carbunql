using Carbunql.Clauses;
using Carbunql.Tables;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryBuilderByLinq.Analysis;

public static class TableInfoParser
{
	public static TableInfo? Parse(Expression exp)
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

	private static TableInfo? ParseCore(Expression exp)
	{
		var from = ParseCore2Arguments(exp);
		if (from != null) return from;
		from = ParseCore3Arguments(exp);
		return from;
	}

	private static TableInfo? ParseCore2Arguments(Expression exp)
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
		return Parse(parameter);
	}

	/// <summary>
	/// Parsing process when the argument is 3.
	/// Analysis processing when using CTE.
	/// </summary>
	/// <param name="exp"></param>
	/// <returns></returns>
	private static TableInfo? ParseCore3Arguments(Expression exp)
	{
		if (exp is not MethodCallExpression) return null;

		var method = (MethodCallExpression)exp;
		if (method.Arguments.Count != 3) return null;

		var ce = method.GetArgument<ConstantExpression>(0);
		var body = method.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>().GetBody<MethodCallExpression>();
		if (body == null) return null;

		if (body.Method.Name == nameof(Sql.FromTable))
		{
			// no relation pattern.
			var operand = method.GetArgument<UnaryExpression>(2).GetOperand<LambdaExpression>();
			if (operand == null) return null;
			if (operand.Parameters.Count != 2) return null;

			var parameter = operand.Parameters[1];
			return ParseAsFromTable(body, parameter);
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
			return Parse(parameter);
		}

		return null;
	}

	public static bool TryParse(ConstantExpression methodBody, ParameterExpression parameter, out TableInfo from)
	{
		from = null!;

		if (methodBody.Value is IQueryable q && q.Provider is TableQuery provider)
		{
			if (provider.InnerQuery != null)
			{
				// subquery pattern.
				from = new TableInfo(provider.InnerQuery.ToSelectQuery(), parameter.Name!);
				return true;
			}
			else
			{
				// override table name pattern.
				from = new TableInfo(provider.TableName, parameter.Name!);
				return true;
			}
		}
		return false;
	}

	public static TableInfo Parse(ParameterExpression parameter)
	{
		return new TableInfo(parameter.ToTypeTable(), parameter.Name!);
	}

	private static TableInfo? ParseAsFromTable(MethodCallExpression body, ParameterExpression parameter)
	{
		if (body.Method.Name != nameof(Sql.FromTable)) throw new InvalidProgramException();

		var alias = parameter.Name!;

		//table
		if (body.Arguments.Count == 0)
		{
			// type table pattern.
			// ex.From<T>()
			return new TableInfo(parameter.ToTypeTable(), alias);
		}

		if (body.Arguments.Count != 1) return null;

		if (body.Arguments[0] is ParameterExpression)
		{
			// single common table pattern.
			// ex.From(IQueryable)
			var prm = body.GetArgument<ParameterExpression>(0);
			if (prm == null) throw new InvalidProgramException();

			var table = prm.Name!;
			var props = prm.Type.GetProperties().Select(x => x.Name).ToList();

			//var t = new TableInfo(table, alias);
			var pt = new PhysicalTable(table) { ColumnNames = props };
			var st = new SelectableTable(pt, alias);
			return new TableInfo(st, alias);
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
				return new TableInfo(m.Member!.Name!, alias);
			}
		}
		else if (body.Arguments[0] is ConstantExpression)
		{
			// string table pattern.
			// ex.From<T>("TableName")
			var table = body.GetArgument<ConstantExpression>(0)?.Value?.ToString()!;
			return new TableInfo(table, alias);
		}

		return null;
	}

	private static TableInfo? ParseAsSubQuery(MemberExpression m, string alias)
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
			return new TableInfo(q.AsQueryable().ToSelectQuery(), alias);
		}
		return null;
	}

	public static SelectableTable ToTypeTable(this ParameterExpression prm)
	{
		var t = new PhysicalTable()
		{
			Table = prm.Type.ToTableName(),
			ColumnNames = prm.Type.GetProperties().ToList().Select(x => x.Name).ToList()
		};
		return t.ToSelectable();
	}
}
