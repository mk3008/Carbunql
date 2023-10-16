using Carbunql;
using Carbunql.Building;
using Carbunql.Values;
using System.Linq.Expressions;

namespace QueryBuilderByLinq;

internal static class SelectQueryExtension
{
	public static SelectQuery AddJoinClause(this SelectQuery sq, LambdaExpression join, List<string> tables)
	{
		var f = sq.FromClause!;

		var me = (MethodCallExpression)join.Body;

		if (me.Method.Name == "InnerJoin")
		{
			var arg = (UnaryExpression)me.Arguments.First();
			var lambda = (LambdaExpression)arg.Operand;

			var table = lambda.Parameters.First();
			var alias = table.Name!;
			var tablename = table.Type.ToTableName();

			var condition = lambda.ToValue(tables);
			f.InnerJoin(tablename).As(alias).On((_) => condition);
			return sq;
		}

		if (me.Method.Name == "LeftJoin")
		{
			var arg = (UnaryExpression)me.Arguments.First();
			var lambda = (LambdaExpression)arg.Operand;

			var table = lambda.Parameters.First();
			var alias = table.Name!;
			var tablename = table.Type.ToTableName();

			var condition = lambda.ToValue(tables);
			f.LeftJoin(tablename).As(alias).On((_) => condition);

			return sq;
		}

		throw new NotSupportedException();
	}

	public static SelectQuery AddSelectClause(this SelectQuery sq, LambdaExpression? select, LambdaExpression? where, List<string> tables)
	{
		if (select != null) return sq.AddSelectClauseBySelectExpression(select, tables);
		if (where != null) return sq.AddSelectClauseByWhereExpression(where, tables);
		return sq;
	}

	private static SelectQuery AddSelectClauseBySelectExpression(this SelectQuery sq, LambdaExpression select, List<string> tables)
	{
		var v = select.Body.ToValue(tables);

		if (v is ValueCollection vc)
		{
			foreach (var item in vc)
			{
				sq.Select(item).As(!string.IsNullOrEmpty(item.RecommendedName) ? item.RecommendedName : item.GetDefaultName());
			}
		}
		else
		{
			sq.Select(v).As(!string.IsNullOrEmpty(v.RecommendedName) ? v.RecommendedName : v.GetDefaultName());
		}
		return sq;
	}

	private static SelectQuery AddSelectClauseByWhereExpression(this SelectQuery sq, LambdaExpression where, List<string> tables)
	{
		var tp = where.Parameters[0].Type;
		var alias = where.Parameters[0].Name!;
		tp.GetProperties().ToList().ForEach(x =>
		{
			sq.Select(alias, x.Name).As(x.Name);
		});
		return sq;
	}
}