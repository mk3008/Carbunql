using Carbunql.Analysis.Parser;
using Carbunql.Clauses;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Building;

public static class WhereClauseExtension
{
	public static ValueBase Where(this SelectQuery source, string table, string column)
	{
		var v = new ColumnValue(table, column);
		return source.Where(v);
	}

	public static ValueBase Where(this SelectQuery source, FromClause table, string column)
	{
		var v = new ColumnValue(table, column);
		return source.Where(v);
	}

	public static ValueBase Where(this SelectQuery source, SelectableTable table, string column)
	{
		var v = new ColumnValue(table, column);
		return source.Where(v);
	}

	public static ValueBase Where(this SelectQuery source, string text)
	{
		var v = ValueParser.Parse(text);
		return source.Where(v);
	}


	public static ValueBase Where(this SelectQuery source, Expression<Func<bool>> predicate)
	{
		var v = ((BinaryExpression)predicate.Body).ToValue();

		if (v is BracketValue)
		{
			source.Where(v);
		}
		else
		{
			source.Where(new BracketValue(v));
		}

		return v;
	}

	public static ValueBase Where(this SelectQuery source, Func<ValueBase> builder)
	{
		var v = builder();
		return source.Where(v);
	}

	public static ValueBase Where(this SelectQuery source, ValueBase value)
	{
		if (source.WhereClause == null)
		{
			source.WhereClause = new WhereClause(value);
		}
		else
		{
			var v = source.WhereClause.Condition.GetLast();
			v.And(value);
		}
		return value;
	}
}