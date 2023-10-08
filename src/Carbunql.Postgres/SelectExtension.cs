using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Postgres;

public static class SelectExtension
{
	public static void SelectAll(this SelectQuery source, Expression<Func<object>> fnc)
	{
		var v = fnc.Compile().Invoke();
		var exp = (UnaryExpression)fnc.Body;
		var op = (MemberExpression)exp.Operand;

		foreach (var prop in v.GetType().GetProperties())
		{
			source.Select(op.Member.Name, prop.Name);
		}
	}

	public static SelectableItem Select(this SelectQuery source, Expression<Func<object>> fnc)
	{
		var tables = source.GetSelectableTables().Select(x => x.Alias).Distinct().ToList();

		var v = fnc.Body.ToValue(tables);
		var item = new SelectableItem(v, v.GetDefaultName());
		source.SelectClause ??= new();
		source.SelectClause.Add(item);
		return item;
	}

	public static ColumnValue GetColumn(this SelectQuery source, Expression<Func<object>> fnc)
	{
		var tables = source.GetSelectableTables().Select(x => x.Alias).Distinct().ToList();
		return (ColumnValue)fnc.Body.ToValue(tables);
	}
}
