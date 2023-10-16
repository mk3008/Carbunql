using Carbunql;
using Carbunql.Building;
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

		var fromTableAlias = string.Empty;
		if (join != null)
		{
			fromTableAlias = join.Parameters.First().Name!;
		}
		else if (select != null)
		{
			fromTableAlias = select.Parameters.First().Name!;
		}
		else if (where != null)
		{
			fromTableAlias = where.Parameters.First().Name!;
		}
		else
		{
			throw new NotSupportedException();
		}

		var tables = (select == null) ? new List<string> { fromTableAlias } : select.Parameters.Select(x => x.Name!).ToList();

		var sq = new SelectQuery();
		sq.From(from.GetTableName()).As(fromTableAlias);

		if (where != null) sq.Where(where.ToValue(tables));
		if (join != null) sq.AddJoinClause(join, tables);

		return sq.AddSelectClause(select, where, tables);
	}

	private SelectQuery BuildAsNest(MethodCallExpression expression, SelectQuery sq)
	{
		var join = GetJoinExpression(expression);
		var select = GetSelectExpression(expression);
		var where = GetWhereExpression(expression);

		var joinAlias = string.Empty;
		if (join != null)
		{
			joinAlias = join.Parameters.First().Name!;
		}
		else if (select != null && select.Parameters.Count == 2)
		{
			joinAlias = select.Parameters[1].Name!;
		}
		else if (where != null)
		{
			joinAlias = where.Parameters.First().Name!;
		}

		var tables = sq.GetSelectableTables().Select(x => x.Alias).ToList();
		if (!string.IsNullOrEmpty(joinAlias)) tables.Add(joinAlias);

		if (join != null && !string.IsNullOrEmpty(joinAlias)) sq.AddJoinClause(join, tables);
		if (where != null) sq.Where(where.ToValue(tables));

		sq.SelectClause = null;
		return sq.AddSelectClause(select, where, tables);
	}
}