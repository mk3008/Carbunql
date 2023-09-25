using Carbunql.Building;
using Carbunql.Clauses;

namespace Carbunql.Postgres;

public static class FromnExtension
{
	public static (FromClause, T) FromAs<T>(this SelectQuery source, string alias)
	{
		var table = typeof(T).ToTableName();
		return source.FromAs<T>(table, alias);
	}

	public static (FromClause, T) FromAs<T>(this SelectQuery source, string table, string alias)
	{
		var r = (T)Activator.CreateInstance(typeof(T))!;
		var (from, _) = source.From(table).As(alias);
		return (from, r);
	}

	public static (FromClause, T) As<T>(this FromClause source, string alias)
	{
		source.As(alias);
		var r = (T)Activator.CreateInstance(typeof(T))!;
		return (source, r);
	}
}
