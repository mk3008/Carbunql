namespace Carbunql;

/// <summary>
/// Provides constants and methods to work with reserved text items commonly used in SQL queries.
/// </summary>
public static class ReservedText
{
    /// <summary>
    /// Represents a comma (`,`).
    /// </summary>
    public static string Comma => ",";

    /// <summary>
    /// Represents a semicolon (`;`).
    /// </summary>
    public static string Semicolon => ";";

    /// <summary>
    /// Represents the SQL keyword `WITH`.
    /// </summary>
    public static string With => "with";

    /// <summary>
    /// Represents the SQL keyword `VALUES`.
    /// </summary>
    public static string Values => "values";

    /// <summary>
    /// Represents the SQL keyword `SELECT`.
    /// </summary>
    public static string Select => "select";

    /// <summary>
    /// Represents the SQL keyword `FROM`.
    /// </summary>
    public static string From => "from";

    /// <summary>
    /// Represents the SQL keyword `WHERE`.
    /// </summary>
    public static string Where => "where";

    /// <summary>
    /// Represents the SQL keyword `GROUP BY`.
    /// </summary>
    public static string Group => "group by";

    /// <summary>
    /// Represents the SQL keyword `HAVING`.
    /// </summary>
    public static string Having => "having";

    /// <summary>
    /// Represents the SQL keyword `WINDOW`.
    /// </summary>
    public static string Window => "window";

    /// <summary>
    /// Represents the SQL keyword `ORDER BY`.
    /// </summary>
    public static string Order => "order by";

    /// <summary>
    /// Represents the SQL keyword `UNION`.
    /// </summary>
    public static string Union => "union";

    /// <summary>
    /// Represents the SQL keyword `UNION ALL`.
    /// </summary>
    public static string UnionAll => "union all";

    /// <summary>
    /// Represents the SQL keyword `MINUS`.
    /// </summary>
    public static string Minus => "minus";

    /// <summary>
    /// Represents the SQL keyword `EXCEPT`.
    /// </summary>
    public static string Except => "except";

    /// <summary>
    /// Represents the SQL keyword `INTERSECT`.
    /// </summary>
    public static string Intersect => "intersect";

    /// <summary>
    /// Represents the SQL keyword `LIMIT`.
    /// </summary>
    public static string Limit => "limit";

    /// <summary>
    /// Represents the SQL keyword `NOT`.
    /// </summary>
    public static string Not => "not";

    /// <summary>
    /// Represents the SQL keyword `AS`.
    /// </summary>
    public static string As => "as";

    /// <summary>
    /// Represents the SQL keyword `MATERIALIZED`.
    /// </summary>
    public static string Materialized => "materialized";

    /// <summary>
    /// Represents the start bracket for SQL expressions (`(`).
    /// </summary>
    public static string StartBracket => "(";

    /// <summary>
    /// Represents the end bracket for SQL expressions (`)`).
    /// </summary>
    public static string EndBracket => ")";

    /// <summary>
    /// Represents the SQL logical operator `AND`.
    /// </summary>
    public static string And => "and";

    /// <summary>
    /// Represents the SQL logical operator `OR`.
    /// </summary>
    public static string Or => "or";

    /// <summary>
    /// Represents the SQL keyword `ON`.
    /// </summary>
    public static string On => "on";

    /// <summary>
    /// Represents the SQL keyword `JOIN`.
    /// </summary>
    public static string Join => "join";

    /// <summary>
    /// Represents the SQL keyword `INNER JOIN`.
    /// </summary>
    public static string Inner => "inner join";

    /// <summary>
    /// Represents the SQL keyword `LEFT JOIN`.
    /// </summary>
    public static string Left => "left join";

    /// <summary>
    /// Represents the SQL keyword `RIGHT JOIN`.
    /// </summary>
    public static string Right => "right join";

    /// <summary>
    /// Represents the SQL keyword `CROSS JOIN`.
    /// </summary>
    public static string Cross => "cross join";

    /// <summary>
    /// Returns an enumerable containing all reserved text items.
    /// </summary>
    /// <returns>An enumerable containing all reserved text items.</returns>
    public static IEnumerable<string> All()
    {
        yield return Comma;
        yield return Semicolon;
        yield return With;
        yield return Values;
        yield return Select;
        yield return From;
        yield return Where;
        yield return Group;
        yield return Having;
        yield return Window;
        yield return Order;
        yield return Union;
        yield return UnionAll;
        yield return Minus;
        yield return Except;
        yield return Intersect;
        yield return Limit;
        yield return Not;
        yield return As;
        yield return Materialized;
        yield return StartBracket;
        yield return EndBracket;
        yield return And;
        yield return Or;
        yield return On;
        yield return Join;
        yield return Inner;
        yield return Left;
        yield return Right;
        yield return Cross;
    }

    /// <summary>
    /// Returns an enumerable containing reserved text items that satisfy the given predicate.
    /// </summary>
    /// <param name="fn">The predicate to filter reserved text items.</param>
    /// <returns>An enumerable containing reserved text items that satisfy the predicate.</returns>
    public static IEnumerable<string> All(Predicate<string> fn)
    {
        foreach (var item in All())
        {
            if (fn(item)) yield return item;
        }
    }

    /// <summary>
    /// Returns an enumerable containing reserved text items related to relations.
    /// </summary>
    /// <returns>An enumerable containing reserved text items related to relations.</returns>
    public static IEnumerable<string> GetRelationTexts()
    {
        yield return Join;
        yield return Inner;
        yield return Left;
        yield return Right;
        yield return Cross;
        yield return Comma;
    }
}
