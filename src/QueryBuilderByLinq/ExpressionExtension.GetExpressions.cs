using Carbunql.Clauses;
using Carbunql.Values;
using Carbunql;
using System.Linq.Expressions;
using Carbunql.Building;

namespace QueryBuilderByLinq;







internal static class LambdaExpressionExpression
{

	internal static ExistsExpression ToExistsExpression(this LambdaExpression predicate, Action<SelectQuery> fromBuilder, string alias, List<string> tables)
	{
		var lst = new List<string>();
		lst.AddRange(tables);
		lst.Add(alias);
		var condition = predicate.Body.ToValue(lst);

		var sq = new SelectQuery();
		fromBuilder(sq);
		sq.SelectAll();

		sq.Where(condition);

		return new ExistsExpression(sq);
	}

	internal static InClause ToInClause(this LambdaExpression predicate, Action<SelectQuery> fromBuilder, string alias, List<string> tables)
	{
		var pv = predicate.Body.ToValue(tables);

		var sq = new SelectQuery();
		fromBuilder(sq);

		var cv = (pv is ColumnValue v) ? v : (pv is BracketValue bv && bv.Inner is ColumnValue bcv) ? bcv : throw new NotSupportedException();

		var lst = ToInClause(cv, alias);

		var args = new ValueCollection();
		foreach (var item in lst)
		{
			args.Add(item.Argument);
			sq.Select(item.Column).As(item.Column.Column);
		}
		ValueBase arg = (args.Count == 1) ? args : args.ToBracket();
		return new InClause(arg, sq.ToValue());
	}

	private static List<(ColumnValue Argument, ColumnValue Column)> ToInClause(ColumnValue predicate, string alias)
	{
		var lst = new List<(ColumnValue Argument, ColumnValue Column)>();

		if (predicate.TableAlias == alias)
		{
			var op = predicate.OperatableValue;
			if (op == null || op.Operator != "=") throw new InvalidProgramException();
			var body = op.Value as ColumnValue;
			if (body == null) throw new NullReferenceException(nameof(body));

			var column = new ColumnValue(predicate.TableAlias, predicate.Column);
			var arg = new ColumnValue(body.TableAlias, body.Column);

			lst.Add((arg, column));

			if (op.Value.OperatableValue != null) lst.AddRange(ToInClause((ColumnValue)op.Value.OperatableValue.Value, alias));
		}
		else
		{
			var op = predicate.OperatableValue;
			if (op == null || op.Operator != "=") throw new InvalidProgramException();
			var body = op.Value as ColumnValue;
			if (body!.TableAlias != alias) throw new InvalidProgramException();

			var arg = new ColumnValue(predicate.TableAlias, predicate.Column);
			var column = new ColumnValue(body.TableAlias, body.Column);

			lst.Add((arg, column));

			if (op.Value.OperatableValue != null) lst.AddRange(ToInClause((ColumnValue)op.Value.OperatableValue.Value, alias));
		}

		return lst;
	}
}
