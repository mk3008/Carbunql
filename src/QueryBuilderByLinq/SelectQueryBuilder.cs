using Carbunql;
using Carbunql.Building;
using Carbunql.Tables;
using Carbunql.Values;
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
			return GetSelectExpression(ue);
		}
		else if (expression.Arguments.Count == 3)
		{
			var ue = (UnaryExpression)expression.Arguments[2];
			return GetSelectExpression(ue);
		}
		throw new NotSupportedException();
	}

	private LambdaExpression? GetSelectExpression(UnaryExpression ue)
	{
		var operand = (LambdaExpression)ue.Operand;
		if (operand.ReturnType == typeof(bool)) return null;
		return operand;
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

	private bool TrySetRootSelectQuery(MethodCallExpression expression, ref SelectQuery sq)
	{
		if (expression.Arguments.Count != 2) return false;

		var fromExpression = (MethodCallExpression)expression.Arguments[0];

		var sqx = BuildAsNest(fromExpression, sq);

		var ue = (UnaryExpression)fromExpression.Arguments[1];
		var selectExpression = (UnaryExpression)fromExpression.Arguments[2];
		var select = selectExpression.Operand as LambdaExpression;

		var lam = (LambdaExpression)ue.Operand;
		if (lam.Body is MethodCallExpression mem2 && mem2.Method.Name == nameof(Sql.FromTable) && mem2.Arguments.Any())
		{
			var val = (ConstantExpression)mem2.Arguments[0];
			var name = val.Value?.ToString();
			if (string.IsNullOrEmpty(name)) return false;
			var table = name;
			var alias = select!.Parameters[1].Name!;

			sq.From(table).As(alias);
			//mem.Arguments[3];

			return true;
		}
		return false;
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
		var root = (ConstantExpression)expression.Arguments[0];
		var nest = GetJoinExpression(expression);
		var select = GetSelectExpression(expression);
		var where = GetWhereExpression(expression);

		if (nest == null)
		{
			return BuildAsRootNoRelation(expression, root, select, where);
		}
		if (where == null && nest != null && nest.Body is MethodCallExpression mc && select != null)
		{
			if (mc.Method.Name == nameof(Sql.FromTable))
			{
				// CTE, From pattern
				return BuildAsRootCte(expression, root, nest, select);
			}
			else if (mc.Method.Name == nameof(Sql.InnerJoinTable) || mc.Method.Name == nameof(Sql.LeftJoinTable) || mc.Method.Name == nameof(Sql.CrossJoinTable))
			{
				return BuildAsRootRelation(expression, root, nest, select, where);
			}
		}

		throw new NotSupportedException();
	}

	private SelectQuery BuildAsRootCte(MethodCallExpression expression, ConstantExpression cte, LambdaExpression from, LambdaExpression select)
	{
		ParameterExpression? fromAlias = null;
		ParameterExpression? joinAlias = null;
		if (select != null)
		{
			fromAlias = select.Parameters.First();
			if (select.Parameters.Count > 1) joinAlias = select.Parameters.Last();
		}
		if (fromAlias == null) throw new NotSupportedException();

		var sq = new SelectQuery();
		if (fromAlias.Type != typeof(object))
		{
			if (cte.Value is IQueryable q && q.Provider is TableQuery tq)
			{
				if (tq.InnerQuery != null)
				{
					sq.With(tq.InnerQuery.ToQueryAsPostgres()).As(fromAlias.Name!);
				}
				else
				{
					throw new NotSupportedException();
				}
			}
		}
		return sq;
	}

	private SelectQuery BuildAsRootRelation(MethodCallExpression expression, ConstantExpression from, LambdaExpression join, LambdaExpression? select, LambdaExpression? where)
	{
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
				if (tq.InnerQuery != null)
				{
					sq.From(tq.InnerQuery.ToQueryAsPostgres()).As(fromAlias.Name!);
				}
				else
				{
					sq.From(tq.TableName).As(fromAlias.Name!);
				}
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

	private SelectQuery BuildAsRootNoRelation(MethodCallExpression expression, ConstantExpression from, LambdaExpression? select, LambdaExpression? where)
	{
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
				if (tq.InnerQuery != null)
				{
					sq.From(tq.InnerQuery.ToQueryAsPostgres()).As(fromAlias.Name!);
				}
				else
				{
					sq.From(tq.TableName).As(fromAlias.Name!);
				}
			}
			else
			{
				sq.From(fromAlias.ToSelectable()).As(fromAlias.Name!);
			}

			if (where != null) sq.Where(where.ToValue(tables));
		}
		return sq.AddSelectClause(select, where, tables);
	}

	private SelectQuery BuildAsNest(MethodCallExpression expression, SelectQuery sq)
	{
		//var from = GetFromTableName(expression);
		var join = GetJoinExpression(expression);
		var select = GetSelectExpression(expression);
		var where = GetWhereExpression(expression);

		if (sq.FromClause == null && (expression.Arguments.Count == 2))
		{
			var fromExpression = (MethodCallExpression)expression.Arguments[0];
			sq = BuildAsNest(fromExpression, sq);


			var from = sq.FromClause!.Root;
			var cols = sq.GetSelectableItems().Where(x => x.Value is ColumnValue).Where(x => from.Alias == ((ColumnValue)x.Value).TableAlias).ToList();
			cols.ForEach(x => sq.SelectClause!.Remove(x));

			var lst = sq.GetSelectableItems().Where(x => x.Value is ColumnValue).Where(x => from.Table.GetTableFullName() == ((ColumnValue)x.Value).TableAlias).ToList();
			if (lst.Any())
			{
				var t = (PhysicalTable)from.Table;
				t.ColumnNames = new List<string>();
				foreach (var item in lst)
				{
					var c = (ColumnValue)item.Value;
					c.TableAlias = from.Alias;
					t.ColumnNames.Add(c.Column);
				}
			}

			var tbls = new List<string> { from.Alias };
			if (where != null) sq.Where(where.ToValue(tbls));
			//sq.AddSelectClause(select, where, tbls);
			var text = sq.ToCommand().CommandText;
			return sq;
		}


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