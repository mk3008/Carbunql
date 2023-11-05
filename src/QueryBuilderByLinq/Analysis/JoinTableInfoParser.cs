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

		var parameter = op.Parameters[0];

		var body = op.GetBody<MethodCallExpression>(); ;
		if (body == null) return null;

		if (ce != null && (body.Method.Name == nameof(Sql.InnerJoinTable) || body.Method.Name == nameof(Sql.LeftJoinTable) || body.Method.Name == nameof(Sql.CrossJoinTable)))
		{
			var operand = method.GetArgument<UnaryExpression>(2).GetOperand<LambdaExpression>();
			if (operand == null || operand.Parameters.Count != 2) return null;



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

			if (TableInfoParser.TryParse(ce, parameter, out var t))
			{
				return new JoinTableInfo(t, body.Method.Name, condition);
			}
			else
			{
				var table = TableInfoParser.Parse(parameter);
				return new JoinTableInfo(table, body.Method.Name, condition);
			}
		}

		return null;
	}
}
