using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql;

public interface IReturning
{
    ReturningClause? ReturningClause { get; set; }
}

public static class IReturningExtension
{
    public static void Returning(this IReturning source, ValueBase value)
    {
        if (source.ReturningClause != null) throw new Exception("returning clause is not empty.");
        source.ReturningClause = new ReturningClause(value);
    }

    public static void Returning(this IReturning source, Func<ValueBase> builder)
    {
        source.Returning(builder());
    }

    public static void Returning(this IReturning source, string columnName)
    {
        source.Returning(new ColumnValue(columnName));
    }

    public static void ReturningAll(this IReturning source)
    {
        source.Returning("*");
    }
}