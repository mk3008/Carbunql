using Carbunql.Values;
using System.Linq.Expressions;

namespace QueryBuilderByLinq.Analysis;

public class JoinTableInfoParser
{
	public static List<JoinTableInfo> Parse(Expression exp)
	{
		var joins = new List<JoinTableInfo>();
		foreach (var item in ExpressionReader.GetExpressions(exp))
		{
			if (TryParse(item, out var j))
			{
				joins.Add(j);
			}
		}
		return joins;
	}

	public static bool TryParse(Expression exp, out JoinTableInfo join)
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
				join = ParseAsCrossJoinTable_string(body, parameter);
				return true;
			}
			throw new NotSupportedException();
		}

		if (body.Method.Name == nameof(Sql.InnerJoinTable) || body.Method.Name == nameof(Sql.LeftJoinTable))
		{
			// method : InnerJoinTable, LeftJoinTable
			if (body.Arguments.Count == 1)
			{
				join = ParseAsJoinTableInfo_expression(body, parameter);
				return true;
			}
			if (body.Arguments.Count == 2)
			{
				join = ParseAsJoinTableInfo_string_expression(body, parameter);
				return true;
			}
			throw new NotSupportedException();
		}

		throw new NotSupportedException();
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
		var table = new TableInfo(tableName, alias.Name!);
		return new JoinTableInfo(table, body.Method.Name);
	}

	private static JoinTableInfo ParseAsJoinTableInfo_expression(MethodCallExpression body, ParameterExpression alias)
	{
		//arg0 : condition, Type : Expression, NodeType : Quote(UnrayEcpression)

		var lambda = body.GetArgument<UnaryExpression>(0).GetOperand<LambdaExpression>();
		if (lambda == null || lambda.Parameters.Count != 1) throw new NotSupportedException();

		var tempName = lambda.Parameters[0].Name!;
		var condition = lambda.ToValue();

		// replace alias name
		foreach (ColumnValue item in condition.GetValues().Where(x => x is ColumnValue c && c.TableAlias == tempName))
		{
			item.TableAlias = alias.Name!;
		}

		var table = TableInfoParser.Parse(alias);
		return new JoinTableInfo(table, body.Method.Name, condition);
	}

	private static JoinTableInfo ParseAsJoinTableInfo_string_expression(MethodCallExpression body, ParameterExpression alias)
	{
		//arg0 : tablename, Type : strig     , NodeType : ConstantExpression
		//arg1 : condition, Type : Expression, NodeType : Quote(UnrayEcpression)

		//arg0
		var tableName = (string)body.GetArgument<ConstantExpression>(0)!.Value!;
		if (string.IsNullOrEmpty(tableName)) throw new NotSupportedException();

		//arg1
		var lambda = body.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>();
		if (lambda == null || lambda.Parameters.Count != 1) throw new NotSupportedException();

		var tempName = lambda.Parameters[0].Name!;
		var condition = lambda.ToValue();

		// replace alias name
		foreach (ColumnValue item in condition.GetValues().Where(x => x is ColumnValue c && c.TableAlias == tempName))
		{
			item.TableAlias = alias.Name!;
		}

		var table = new TableInfo(tableName, alias.Name!);
		return new JoinTableInfo(table, body.Method.Name, condition);
	}
}
