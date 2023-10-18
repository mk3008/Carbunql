using Carbunql;
using Carbunql.Building;
using Carbunql.Tables;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

public class SelectQueryBuilder
{
	public SelectQueryBuilder(MethodCallExpression expression)
	{
		Expression = expression;
	}

	public MethodCallExpression Expression { get; init; }

	private LambdaExpression? GetSelectExpression(MethodCallExpression expression)
	{
		if (expression.Arguments.Count == 2)
		{
			var ue = (UnaryExpression)expression.Arguments[1];
			var operand = (LambdaExpression)ue.Operand;
			if (operand.ReturnType == typeof(bool)) return null;
			return operand;
		}
		else if (expression.Arguments.Count == 3)
		{
			var ue = (UnaryExpression)expression.Arguments[2];
			var operand = (LambdaExpression)ue.Operand;
			if (operand.ReturnType == typeof(bool)) return null;
			return operand;
		}
		throw new NotSupportedException();
	}

	//private LambdaExpression? GetJoinExpression(MethodCallExpression expression)
	//{
	//	if (expression.Arguments.Count == 3)
	//	{
	//		var ue = (UnaryExpression)expression.Arguments[1];
	//		return (LambdaExpression)ue.Operand;
	//	}
	//	return null;
	//}

	private LambdaExpression? GetJoinExpression(MethodCallExpression expression)
	{
		if (expression.Arguments.Count == 3)
		{
			var ue = (UnaryExpression)expression.Arguments[1];
			return (LambdaExpression)ue.Operand;
		}
		return null;
	}

	private LambdaExpression? GetWhereExpression(MethodCallExpression expression)
	{
		if (expression.Arguments.Count == 2)
		{
			var ue = (UnaryExpression)expression.Arguments[1];
			var operand = (LambdaExpression)ue.Operand;
			if (operand.ReturnType != typeof(bool)) return null;
			return operand;
		}
		return null;
	}

	public SelectQuery Build(MethodCallExpression expression)
	{
		if (expression.Arguments[0] is ConstantExpression)
		{
			return BuildAsRoot(expression);
		}
		else if (expression.Arguments[0] is MethodCallExpression mce)
		{
			var sq = Build(mce);
			var x = BuildAsNest(expression, sq);
			return sq;
		}

		throw new NotSupportedException();
	}

	private SelectQuery BuildAsRoot(MethodCallExpression expression)
	{
		var from = (ConstantExpression)expression.Arguments[0];
		var join = GetJoinExpression(expression);
		var select = GetSelectExpression(expression);
		var where = GetWhereExpression(expression);

		ParameterExpression? fromAlias = null;
		ParameterExpression? joinAlias = null;
		if (select != null)
		{
			fromAlias = select.Parameters.First();
			if (select.Parameters.Count > 1) joinAlias = select.Parameters.Last();
		}
		else if (where != null)
		{
			fromAlias = where.Parameters.First();
		}
		if (fromAlias == null) throw new NotSupportedException();

		var tables = new List<string> { fromAlias.Name! };

		var sq = new SelectQuery();
		if (fromAlias.Type != typeof(object))
		{
			if (from.Value is IQueryable q && q.Provider is TableQuery tq)
			{
				sq.From(tq.TableName).As(fromAlias.Name!);
			}
			else
			{
				sq.From(fromAlias.ToSelectable()).As(fromAlias.Name!);
			}

			if (where != null) sq.Where(where.ToValue(tables));
			if (join != null && joinAlias != null)
			{
				sq.AddJoinClause(join, tables, joinAlias);
			}
		}
		return sq.AddSelectClause(select, where, tables);
	}

	private SelectQuery BuildAsNest(MethodCallExpression expression, SelectQuery sq)
	{
		var join = GetJoinExpression(expression);
		var select = GetSelectExpression(expression);
		var where = GetWhereExpression(expression);

		ParameterExpression? joinAlias = null;
		if (select != null && select.Parameters.Count > 1)
		{
			joinAlias = select.Parameters.Last();
		}
		else if (where != null)
		{
			joinAlias = where.Parameters.First();
		}

		var tables = sq.GetSelectableTables().Select(x => x.Alias).ToList();
		if (joinAlias != null) tables.Add(joinAlias.Name!);

		if (join != null && joinAlias != null)
		{
			sq.AddJoinClause(join, tables, joinAlias);
		}
		if (where != null) sq.Where(where.ToValue(tables));

		sq.SelectClause = null;
		return sq.AddSelectClause(select, where, tables);
	}
}