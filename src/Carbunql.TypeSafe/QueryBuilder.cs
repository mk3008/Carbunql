using Carbunql.Annotations;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe;

public class QueryBuilder(ISqlDialect sqlDialect)
{
    public readonly ISqlDialect SqlDialect = sqlDialect;

    public FluentSelectQuery<T> From<T>(Expression<Func<T>> expression) where T : IDataRow, new()
    {
        var sq = new FluentSelectQuery<T>();

        var alias = ((MemberExpression)expression.Body).Member.Name;

        //execute
        var compiledExpression = expression.Compile();
        var result = compiledExpression();

        result.DataSet.BuildFromClause(sq, alias);

        return sq;
    }

    public static bool Exists<T>(Func<T> getDataSet, Expression<Func<T, bool>> expression) where T : IDataRow, new()
    {
        return true;
    }

    public static bool NotExists<T>(Func<T> getDataSet, Expression<Func<T, bool>> expression) where T : IDataRow, new()
    {
        return true;
    }


    public static string Raw(string command)
    {
        return command;
    }

    public static string CurrentTimestamp => string.Empty;

    public static DateTime Now => new DateTime();

    public static DateTime DateTruncateToYear(DateTime d) => new DateTime();

    public static DateTime DateTruncateToQuarter(DateTime d) => new DateTime();

    public static DateTime DateTruncToMonth(DateTime d) => new DateTime();

    public static DateTime DateTruncateToDay(DateTime d) => new DateTime();

    public static DateTime DateTruncateToHour(DateTime d) => new DateTime();

    public static DateTime DateTruncateToMinute(DateTime d) => new DateTime();

    public static DateTime DateTruncateToSecond(DateTime d) => new DateTime();

    public static int RowNumber() => 0;

    public static int RowNumber(Expression<Func<object?>> partition, Expression<Func<object?>> order) => throw new InvalidProgramException();

    public static int Count() => throw new InvalidProgramException();

    public static int Count(Expression<Func<object?>> partition, Expression<Func<object?>> order) => throw new InvalidProgramException();

    public static T Sum<T>(Expression<Func<T>> expression) => throw new InvalidProgramException();

    public static T Sum<T>(Expression<Func<T>> expression, Expression<Func<object?>> partition, Expression<Func<object?>> order) => throw new InvalidProgramException();

    public static T Average<T>(Expression<Func<T>> expression) => throw new InvalidProgramException();

    public static T Average<T>(Expression<Func<T>> expression, Expression<Func<object?>> partition, Expression<Func<object?>> order) => throw new InvalidProgramException();

    public static T Max<T>(Expression<Func<T>> expression) => throw new InvalidProgramException();

    public static T Max<T>(Expression<Func<T>> expression, Expression<Func<object?>> partition, Expression<Func<object?>> order) => throw new InvalidProgramException();

    public static T Min<T>(Expression<Func<T>> expression) => throw new InvalidProgramException();

    public static T Min<T>(Expression<Func<T>> expression, Expression<Func<object?>> partition, Expression<Func<object?>> order) => throw new InvalidProgramException();

}
