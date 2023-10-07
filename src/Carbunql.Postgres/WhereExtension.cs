using Carbunql.Building;
using Carbunql.Clauses;
using System.Linq.Expressions;

namespace Carbunql.Postgres;

public static class WhereExtension
{
	public static ValueBase Where(this SelectQuery source, Expression<Func<bool>> predicate)
	{
		var tables = source.GetSelectableTables().Select(x => x.Alias).Distinct().ToList();
		var v = predicate.Body.ToValue(tables);

		source.Where(v);

		return v;
	}
}
