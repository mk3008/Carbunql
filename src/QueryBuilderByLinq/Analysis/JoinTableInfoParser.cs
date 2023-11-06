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
			var j = ParseCore(item);
			if (j != null) joins.Add(j);
		}
		return joins;
	}

	private static JoinTableInfo? ParseCore(Expression exp)
	{
		if (exp is not MethodCallExpression) return null;

		var method = (MethodCallExpression)exp;
		if (method.Arguments.Count != 3) return null;

		var ce = method.GetArgument<ConstantExpression>(0);
		var op = method.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>();
		if (op == null || op.Parameters.Count() != 1) return null;

		//var parameter = op.Parameters[0];

		var body = op.GetBody<MethodCallExpression>(); ;
		if (body == null) return null;

		//if (ce != null && (body.Method.Name == nameof(Sql.InnerJoinTable) || body.Method.Name == nameof(Sql.LeftJoinTable) || body.Method.Name == nameof(Sql.CrossJoinTable)))
		//{
		var operand = method.GetArgument<UnaryExpression>(2).GetOperand<LambdaExpression>();
		if (operand == null || operand.Parameters.Count != 2) return null;
		var parameter = operand.Parameters[1];

		if (body.Method.Name == nameof(Sql.CrossJoinTable))
		{
			// method : CrossJoinTable
			if (body.Arguments.Count == 0)
			{
				// no argument.

				var table = TableInfoParser.Parse(parameter);
				return new JoinTableInfo(table, body.Method.Name);
			}
			else if (body.Arguments.Count == 1 && body.GetArgument<ConstantExpression>(0)?.Type == typeof(string))
			{
				//arg0 : tablename, Type : strig, NodeType : ConstantExpression
				var tableName = (string)body.GetArgument<ConstantExpression>(0)!.Value!;
				var table = new TableInfo(tableName, parameter.Name!);
				return new JoinTableInfo(table, body.Method.Name);
			}
			throw new NotSupportedException();
		}
		else if (body.Arguments.Count == 2)
		{
			//method : InnerJoinTable, LeftJoinTable
			//arg0   : tablename, Type : strig     , NodeType : ConstantExpression
			//arg1   : condition, Type : Expression, NodeType : Quote(UnrayEcpression)

			var lambda = body.GetArgument<UnaryExpression>(1).GetOperand<LambdaExpression>();
			if (lambda == null || lambda.Parameters.Count != 1) return null;

			var arg0 = body.GetArgument<ConstantExpression>(0);


			var tempName = lambda.Parameters[0].Name!;
			var actualName = operand.Parameters[1].Name!;
			var condition = lambda.ToValue();

			// replace alias name
			foreach (ColumnValue item in condition.GetValues().Where(x => x is ColumnValue c && c.TableAlias == tempName))
			{
				item.TableAlias = actualName;
			}

			//if (TableInfoParser.TryParse(ce, parameter, out var t))
			//{
			//	return new JoinTableInfo(t, body.Method.Name, condition);
			//}
			if (arg0 != null && arg0.Type == typeof(string))
			{
				// override tablename pattern.
				var tableName = (string)arg0.Value!;
				if (string.IsNullOrEmpty(tableName)) throw new InvalidProgramException();
				var table = new TableInfo(tableName, parameter.Name!);
				return new JoinTableInfo(table, body.Method.Name, condition);
			}
			else
			{
				var table = TableInfoParser.Parse(operand.Parameters[1]);
				return new JoinTableInfo(table, body.Method.Name, condition);
			}

		}
		else if (body.Arguments.Count == 1)
		{
			//method : CrossJoinTable
			//arg0   : tablename, Type : strig     , NodeType : ConstantExpression

			var lambda = body.GetArgument<UnaryExpression>(0).GetOperand<LambdaExpression>();
			if (lambda == null || lambda.Parameters.Count != 1) return null;

			var tempName = lambda.Parameters[0].Name!;
			var actualName = operand.Parameters[1].Name!;
			var condition = lambda.ToValue();

			// replace alias name
			foreach (ColumnValue item in condition.GetValues().Where(x => x is ColumnValue c && c.TableAlias == tempName))
			{
				item.TableAlias = actualName;
			}

			//if (TableInfoParser.TryParse(ce, parameter, out var t))
			//{
			//	return new JoinTableInfo(t, body.Method.Name, condition);
			//}
			//else
			{
				var table = TableInfoParser.Parse(operand.Parameters[1]);
				return new JoinTableInfo(table, body.Method.Name, condition);
			}
		}
		//}

		return null;
	}
}
