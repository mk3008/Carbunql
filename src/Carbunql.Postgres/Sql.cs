using Carbunql.Clauses;
using Carbunql.Values;
using System.Linq.Expressions;

namespace Carbunql.Postgres;

/// <summary>
/// Only function definitions are written for use in expression trees.
/// The actual situation is in ExpressionExtension.cs.
/// </summary>
public static class Sql
{
	private static string ERROR = "Definition methods must not be executed.";

	public static bool ExistsAs<T>(this SelectQuery source, string table, Expression<Func<T, bool>> predicate)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static bool ExistsAs<T>(this SelectQuery source, IReadQuery subQuery, Expression<Func<T, bool>> predicate)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static bool ExistsAs<T>(this SelectQuery source, Expression<Func<T, bool>> predicate)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static bool InAs<T>(this SelectQuery source, string table, Expression<Func<T, bool>> predicate)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static bool InAs<T>(this SelectQuery source, IReadQuery subQuery, Expression<Func<T, bool>> predicate)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static bool InAs<T>(this SelectQuery source, Expression<Func<T, bool>> predicate)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static ValueBase Greatest(this SelectQuery source, Expression<Func<IEnumerable<object>>> args)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static ValueBase Greatest(this SelectQuery source, Expression<Func<IEnumerable<ValueBase>>> args)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static ValueBase Least(this SelectQuery source, Expression<Func<IEnumerable<object>>> predicate)
	{
		throw new InvalidProgramException(ERROR);
	}

	public static ValueBase Least(this SelectQuery source, Expression<Func<IEnumerable<ValueBase>>> args)
	{
		throw new InvalidProgramException(ERROR);
	}
}