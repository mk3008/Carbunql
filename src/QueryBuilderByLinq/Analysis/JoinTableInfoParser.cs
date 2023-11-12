using Carbunql.Tables;
using Carbunql.Values;
using System.Linq.Expressions;

namespace QueryBuilderByLinq.Analysis;

public class JoinTableInfoParser
{
	public static List<JoinTableInfo> Parse(Expression exp)
	{
		var tables = new List<string>();
		if (TableInfoParser.TryParse(exp, out var t))
		{
			tables.Add(t.Alias);
		}

		var joins = new List<JoinTableInfo>();
		foreach (var item in ExpressionReader.GetExpressions(exp).Reverse())
		{
			if (TryParse(item, tables, out var j))
			{
				joins.Add(j);
				tables.Add(j.TableInfo.Alias);
			}
		}
		return joins;
	}

	public static bool TryParse(Expression exp, List<string> tables, out JoinTableInfo join)
	{
		join = null!;

		if (exp is not MethodCallExpression) return false;

		var method = (MethodCallExpression)exp;
		if (method.Arguments.Count != 3) return false;

		var op = method.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>();
		if (op == null || op.Parameters.Count() != 1) return false;

		var body = op.GetBody<MethodCallExpression>(); ;
		if (body == null) return false;

		var operand = method.GetArgument<UnaryExpression>(2).GetOperand<LambdaExpression>();
		if (operand == null || operand.Parameters.Count != 2) return false;

		var parameter = operand.Parameters[1];

		if (body.Method.Name == nameof(Sql.CrossJoinTable))
		{
			// method : CrossJoinTable
			if (body.Arguments.Count == 0)
			{
				join = ParseAsCrossJoinTable(body, parameter);
				return true;
			}
			else if (body.Arguments.Count == 1 && body.GetArgument<ConstantExpression>(0)?.Type == typeof(string))
			{
				// override table name
				join = ParseAsCrossJoinTable_string(body, parameter);
				return true;
			}
			else if (body.Arguments.Count == 1 && body.Arguments[0] is MemberExpression)
			{
				// subquery
				join = Parse(body, (MemberExpression)body.Arguments[0], parameter, tables);
				return true;
			}
			throw new NotSupportedException();
		}

		if (body.Method.Name == nameof(Sql.InnerJoinTable) || body.Method.Name == nameof(Sql.LeftJoinTable))
		{
			// method : InnerJoinTable, LeftJoinTable
			if (body.Arguments.Count == 1)
			{
				join = ParseAsJoinTableInfo_expression(body, parameter, tables);
				return true;
			}
			if (body.Arguments.Count == 2)
			{
				if (body.Arguments[0] is ConstantExpression && body.Arguments[1] is UnaryExpression)
				{
					join = Parse(body, (ConstantExpression)body.Arguments[0], (UnaryExpression)body.Arguments[1], parameter, tables);
					return true;
				}
				else if (body.Arguments[0] is MemberExpression && body.Arguments[1] is UnaryExpression)
				{
					join = Parse(body, (MemberExpression)body.Arguments[0], (UnaryExpression)body.Arguments[1], parameter, tables);
					return true;
				}
				throw new NotSupportedException();
			}
			throw new NotSupportedException();
		}

		return false;
	}

	private static JoinTableInfo ParseAsCrossJoinTable(MethodCallExpression body, ParameterExpression alias)
	{
		var table = TableInfoParser.Parse(alias);
		return new JoinTableInfo(table, body.Method.Name);
	}

	private static JoinTableInfo ParseAsCrossJoinTable_string(MethodCallExpression body, ParameterExpression alias)
	{
		//arg0 : tablename, Type : strig, NodeType : ConstantExpression

		var tableName = (string)body.GetArgument<ConstantExpression>(0)!.Value!;
		var table = CreateTableInfo(tableName, alias);
		return new JoinTableInfo(table, body.Method.Name);
	}

	private static JoinTableInfo ParseAsJoinTableInfo_expression(MethodCallExpression body, ParameterExpression alias, List<string> tables)
	{
		//arg0 : condition, Type : Expression, NodeType : Quote(UnrayEcpression)

		var lambda = body.GetArgument<UnaryExpression>(0).GetOperand<LambdaExpression>();
		if (lambda == null || lambda.Parameters.Count != 1) throw new NotSupportedException();

		var tempName = lambda.Parameters[0].Name!;
		var condition = lambda.ToValue(tables);

		// replace alias name
		foreach (ColumnValue item in condition.GetValues().Where(x => x is ColumnValue c && c.TableAlias == tempName))
		{
			item.TableAlias = alias.Name!;
		}

		var table = TableInfoParser.Parse(alias);
		return new JoinTableInfo(table, body.Method.Name, condition);
	}

	private static JoinTableInfo Parse(MethodCallExpression body, ConstantExpression arg0, UnaryExpression arg1, ParameterExpression alias, List<string> tables)
	{
		//arg0 : tablename, Type : strig     , NodeType : ConstantExpression
		//arg1 : condition, Type : Expression, NodeType : Quote(UnrayEcpression)

		//arg0
		var tableName = (string)arg0.Value!;
		if (string.IsNullOrEmpty(tableName)) throw new NotSupportedException();

		//arg1
		var lambda = arg1.GetOperand<LambdaExpression>();
		if (lambda == null || lambda.Parameters.Count != 1) throw new NotSupportedException();

		var paremeter = lambda.GetParameter<ParameterExpression>(0);
		if (paremeter == null) throw new NotSupportedException();

		var condition = lambda.ToValue(tables);

		// replace alias name
		foreach (ColumnValue item in condition.GetValues().Where(x => x is ColumnValue c && c.TableAlias == paremeter.Name!))
		{
			item.TableAlias = alias.Name!;
		}

		var table = CreateTableInfo(tableName, alias);//, paremeter);
		return new JoinTableInfo(table, body.Method.Name, condition);
	}

	private static JoinTableInfo Parse(MethodCallExpression body, MemberExpression arg0, ParameterExpression alias, List<string> tables)
	{
		//arg0
		if (TableInfoParser.TryParse(arg0, alias.Name!, out var table))
		{
			return new JoinTableInfo(table, body.Method.Name);
		}
		throw new NotSupportedException();
	}

	private static JoinTableInfo Parse(MethodCallExpression body, MemberExpression arg0, UnaryExpression arg1, ParameterExpression alias, List<string> tables)
	{
		//arg0
		TableInfoParser.TryParse(arg0, alias.Name!, out var table);
		if (table == null) throw new NotSupportedException();

		//arg1
		var lambda = arg1.GetOperand<LambdaExpression>();
		if (lambda == null || lambda.Parameters.Count != 1) throw new NotSupportedException();

		var paremeter = lambda.GetParameter<ParameterExpression>(0);
		if (paremeter == null) throw new NotSupportedException();

		var condition = lambda.ToValue(tables);

		return new JoinTableInfo(table, body.Method.Name, condition);
	}

	private static TableInfo CreateTableInfo(string tableName, ParameterExpression alias)//, ParameterExpression parameter)
	{
		var pt = new PhysicalTable(tableName) { ColumnNames = alias.Type.GetProperties().Select(x => x.Name).ToList() };
		var table = new TableInfo(pt.ToSelectable(alias.Name!));
		return table;
	}
}
