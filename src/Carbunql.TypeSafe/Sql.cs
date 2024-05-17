using Carbunql.Analysis.Parser;
using Carbunql.Annotations;
using Carbunql.Building;
using System.Data;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe;


/// <summary>
/// Only function definitions are written for use in expression trees.
/// The actual situation is in ExpressionExtension.cs.
/// </summary
public static class Sql
{
    public static T DefineTable<T>() where T : ITableRowDefinition, new()
    {
        var instance = new T();
        instance.TableDefinition = TableDefinitionClauseFactory.Create<T>();
        return instance;
    }

    public static FluentSelectQuery From<T>(Expression<Func<T>> expression) where T : ITableRowDefinition
    {
        var sq = new FluentSelectQuery();

        var alias = ((MemberExpression)expression.Body).Member.Name;

        //execute
        var compiledExpression = expression.Compile();
        var result = compiledExpression();

        sq.From(result.TableDefinition).As(alias);

        return sq;
    }

    public static string Raw(string command)
    {
        return command;
    }

    public static string CurrentTimestamp => string.Empty;

    public static DateTime Now => new DateTime();

    public static DateTime DateTruncYear(DateTime d) => new DateTime();

    public static DateTime DateTruncQuarter(DateTime d) => new DateTime();

    public static DateTime DateTruncMonth(DateTime d) => new DateTime();

    public static DateTime DateTruncDay(DateTime d) => new DateTime();

    public static DateTime DateTruncHour(DateTime d) => new DateTime();

    public static DateTime DateTruncMinute(DateTime d) => new DateTime();

    public static DateTime DateTruncSecond(DateTime d) => new DateTime();

    public static string RowNumber() => string.Empty;

    public static string RowNumber(object partition, object order) => string.Empty;

    public static string RowNumberPartitionBy(object partition) => string.Empty;

    public static string RowNumberOrderbyBy(object order) => string.Empty;
}
