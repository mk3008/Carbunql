using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Postgres;
using Carbunql.Tables;
using System.Linq.Expressions;
using System.Reflection;

namespace Carbunql.Postgres.Linq;

public static class SelectableTableParser
{
	public static SelectableTable Parse(Expression exp)
	{
		if (TryParse(exp, out SelectableTable tableInfo))
		{
			return tableInfo;
		}
		throw new NotSupportedException();
	}

	public static bool TryParse(Expression exp, out SelectableTable info)
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

	private static bool TryParseCore(Expression exp, out SelectableTable info)
	{
		info = null!;

		if (exp is not MethodCallExpression) return false;

		var method = (MethodCallExpression)exp;

		if (method.Arguments.Count == 2 && method.Arguments[0] is ConstantExpression && (method.Method.Name == "Select" || method.Method.Name == "Where"))
		{
			//root from pattern.
			var ce = (ConstantExpression)method.Arguments[0];

			var operand = method.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>();
			if (operand == null) throw new NotSupportedException();
			if (operand.Parameters.Count != 1) throw new NotSupportedException();

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
			var body = method.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>().GetBody<MethodCallExpression>();
			if (body == null) return false;

			if (body.Method.Name == nameof(Sql.FromTable))
			{
				// no relation pattern.
				var operand = method.GetArgument<UnaryExpression>(2).GetOperand<LambdaExpression>();
				if (operand == null) return false;
				if (operand.Parameters.Count == 2)
				{
					var parameter = operand.Parameters[1];
					if (TryParseAsFromTable(body, parameter, out info)) return true;
					throw new NotSupportedException();

				}
				throw new NotSupportedException();
			}

			if (method.Arguments[0] is ConstantExpression ce && (body.Method.Name == nameof(Sql.InnerJoinTable) || body.Method.Name == nameof(Sql.LeftJoinTable) || body.Method.Name == nameof(Sql.CrossJoinTable)))
			{
				//root
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

			return false;
		}
		return false;
	}

	public static bool TryParse(ConstantExpression methodBody, ParameterExpression parameter, out SelectableTable from)
	{
		from = null!;

		if (methodBody.Value is IQueryable q && q.Provider is TableQuery provider)
		{
			if (provider.InnerQuery != null)
			{
				// subquery pattern.
				var sq = provider.InnerQuery.ToSelectQuery();
				from = new VirtualTable(sq).ToSelectable(parameter.Name!);
				return true;
			}
			else if (provider.InnerSelectQuery != null)
			{
				// selectquery pattern.
				var sq = provider.InnerSelectQuery;
				from = new VirtualTable(sq).ToSelectable(parameter.Name!);
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

	private static bool TryParseAsFromTable(MethodCallExpression body, ParameterExpression parameter, out SelectableTable info)
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
			else if (m.Expression is MemberExpression)
			{
				// many common tables pattern.(There are 3 or more common tables)
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
	public static SelectableTable Parse(ParameterExpression parameter)
	{
		return Parse(parameter, parameter.Type.ToTableName());
	}

	/// <summary>
	/// Parse types as table information
	/// </summary>
	/// <param name="parameter"></param>
	/// <returns></returns>
	public static SelectableTable Parse(ParameterExpression parameter, string tablenaem)
	{
		var pt = new PhysicalTable(tablenaem)
		{
			ColumnNames = parameter.Type.GetProperties().Select(x => x.Name).ToList()
		};
		var info = pt.ToSelectable(parameter.Name!);
		return info;
	}

	/// <summary>
	/// Parse subquery as table information
	/// </summary>
	/// <param name="table"></param>
	/// <param name="alias"></param>
	/// <returns></returns>
	public static SelectableTable Parse(ParameterExpression table, ParameterExpression alias)
	{
		var pt = new PhysicalTable(table.Name!)
		{
			ColumnNames = table.Type.GetProperties().Select(x => x.Name).ToList()
		};
		var info = pt.ToSelectable(alias.Name!);
		return info;
	}

	public static SelectableTable Parse(MemberExpression cte, ParameterExpression alias)
	{
		if (cte.Type == typeof(SelectQuery))
		{
			//select qurey
			var sq = (SelectQuery)Expression.Lambda(cte).Compile().DynamicInvoke()!;
			return sq.ToSelectableTable(alias.Name!);
		}

		// many common tables pattern.
		// ex.From(IQueryable)
		//return new TableInfo(m.Member!.Name!, alias);
		var pt = new PhysicalTable(cte.Member!.Name!)
		{
			ColumnNames = cte.Type.GetProperties().Select(p => p.Name).ToList()
		};
		var info = pt.ToSelectable(alias.Name!);
		return info;
	}

	public static bool TryParse(MemberExpression m, string alias, out SelectableTable info)
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
			info = new VirtualTable(sq).ToSelectable(alias);
			return true;
		}
		return false;
	}
}
