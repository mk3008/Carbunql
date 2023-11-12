using Carbunql.Tables;
using System.Linq.Expressions;
using System.Reflection;

namespace QueryBuilderByLinq.Analysis;

public static class TableInfoParser
{
	public static TableInfo Parse(Expression exp)
	{
		if (TryParse(exp, out TableInfo tableInfo))
		{
			return tableInfo;
		}
		throw new NotSupportedException();
	}

	public static bool TryParse(Expression exp, out TableInfo info)
	{
		info = null!;

		foreach (var item in exp.GetExpressions().ToList())
		{
			if (TryParseCore(item, out info))
			{
				return true;
			}
		}
		return false;
	}

	private static bool TryParseCore(Expression exp, out TableInfo info)
	{
		info = null!;

		if (exp is not MethodCallExpression) return false;

		var method = (MethodCallExpression)exp;
		if (method.Arguments.Count == 2)
		{
			var ce = method.GetArgument<ConstantExpression>(0);
			if (ce == null) return false;

			var operand = method.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>();
			if (operand == null) return false;
			if (operand.Parameters.Count != 1) return false;

			var parameter = operand.Parameters[0];
			if (parameter.Type == typeof(DualTable)) return false;

			if (TryParse(ce, parameter, out info))
			{
				return true;
			}
			// nest sytax
			info = Parse(parameter);
			return true;
		}
		if (method.Arguments.Count == 3)
		{
			var ce = method.GetArgument<ConstantExpression>(0);
			var body = method.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>().GetBody<MethodCallExpression>();
			if (body == null) return false;

			if (body.Method.Name == nameof(Sql.FromTable))
			{
				// no relation pattern.
				var operand = method.GetArgument<UnaryExpression>(2).GetOperand<LambdaExpression>();
				if (operand == null) return false;
				if (operand.Parameters.Count != 2) return false;

				var parameter = operand.Parameters[1];
				if (TryParseAsFromTable(body, parameter, out info)) return true;
				return false;
			}
			else if (ce != null && (body.Method.Name == nameof(Sql.InnerJoinTable) || body.Method.Name == nameof(Sql.LeftJoinTable) || body.Method.Name == nameof(Sql.CrossJoinTable)))
			{
				// has relation pattern.
				var operand = method.GetArgument<UnaryExpression>(2).GetOperand<LambdaExpression>();
				if (operand == null || operand.Parameters.Count != 2) return false;

				var parameter = operand.Parameters[0];

				if (TryParse(ce, parameter, out info))
				{
					return true;
				}
				info = Parse(parameter);
				return true;
			}
		}
		return false;
	}

	public static bool TryParse(ConstantExpression methodBody, ParameterExpression parameter, out TableInfo from)
	{
		from = null!;

		if (methodBody.Value is IQueryable q && q.Provider is TableQuery provider)
		{
			if (provider.InnerQuery != null)
			{
				// subquery pattern.
				var sq = provider.InnerQuery.ToSelectQuery();
				from = new TableInfo(new VirtualTable(sq).ToSelectable(parameter.Name!));
				return true;
			}
			else
			{
				// override table name pattern.
				from = Parse(parameter, provider.TableName);
				return true;
			}
		}
		return false;
	}

	private static bool TryParseAsFromTable(MethodCallExpression body, ParameterExpression parameter, out TableInfo info)
	{
		info = null!;

		if (body.Method.Name != nameof(Sql.FromTable)) throw new InvalidProgramException();

		var alias = parameter.Name!;

		//table
		if (body.Arguments.Count == 0)
		{
			// type table pattern.
			// ex.From<T>()
			info = Parse(parameter);
			return true;
		}

		if (body.Arguments.Count != 1) return false;

		if (body.Arguments[0] is ParameterExpression)
		{
			// single common table pattern.
			// ex.From(IQueryable)
			//return Parse((ParameterExpression)body.Arguments[0]);

			var table = body.GetArgument<ParameterExpression>(0)!;
			info = Parse(table, parameter);
			return true;
		}
		else if (body.Arguments[0] is MemberExpression)
		{
			var m = (MemberExpression)body.Arguments[0];
			if (m.Expression is ConstantExpression)// && lambda.Parameters.Count == 1
			{
				// subquery pattern.
				if (TryParse(m, alias, out info)) return true;
				return false;
			}
			else if (m.Expression is ParameterExpression)
			{
				// many common tables pattern.
				// ex.From(IQueryable)
				info = Parse(cte: m, alias: parameter);
				return true;
			}
		}
		else if (body.Arguments[0] is ConstantExpression ce && ce.Type == typeof(string))
		{
			// string table pattern.
			// ex.From<T>("TableName")
			var tablename = (string)ce.Value!;
			info = Parse(parameter, tablename);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Parse types as table information
	/// </summary>
	/// <param name="parameter"></param>
	/// <returns></returns>
	public static TableInfo Parse(ParameterExpression parameter)
	{
		return Parse(parameter, parameter.Type.ToTableName());
	}

	/// <summary>
	/// Parse types as table information
	/// </summary>
	/// <param name="parameter"></param>
	/// <returns></returns>
	public static TableInfo Parse(ParameterExpression parameter, string tablenaem)
	{
		var pt = new PhysicalTable(tablenaem)
		{
			ColumnNames = parameter.Type.GetProperties().Select(x => x.Name).ToList()
		};
		var info = new TableInfo(pt.ToSelectable(parameter.Name!));
		return info;
	}

	/// <summary>
	/// Parse subquery as table information
	/// </summary>
	/// <param name="table"></param>
	/// <param name="alias"></param>
	/// <returns></returns>
	public static TableInfo Parse(ParameterExpression table, ParameterExpression alias)
	{
		var pt = new PhysicalTable(table.Name!)
		{
			ColumnNames = table.Type.GetProperties().Select(x => x.Name).ToList()
		};
		var info = new TableInfo(pt.ToSelectable(alias.Name!));
		return info;
	}

	public static TableInfo Parse(MemberExpression cte, ParameterExpression alias)
	{
		// many common tables pattern.
		// ex.From(IQueryable)
		//return new TableInfo(m.Member!.Name!, alias);
		var pt = new PhysicalTable(cte.Member!.Name!)
		{
			ColumnNames = cte.Type.GetProperties().Select(p => p.Name).ToList()
		};
		var info = new TableInfo(pt.ToSelectable(alias.Name!));
		return info;
	}

	public static bool TryParse(MemberExpression m, string alias, out TableInfo info)
	{
		info = null!;

		if (m.Expression is not ConstantExpression) return false;

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
			var sq = q.AsQueryable().ToSelectQuery();
			info = new TableInfo(new VirtualTable(sq).ToSelectable(alias));
			return true;
		}
		return false;
	}
}
