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
    public static T DefineTable<T>(SelectQuery query) where T : ITableRowDefinition, new()
    {
        var instance = new T();

        var info = TableInfoFactory.Create(typeof(T));

        instance.CreateTableQuery = query.ToCreateTableQuery("_" + info.Table, isTemporary: true);
        return instance;
    }

    public static T DefineTable<T>(FluentSelectQuery<T> query) where T : ITableRowDefinition, new()
    {
        var instance = new T();

        var info = TableInfoFactory.Create(typeof(T));

        instance.CreateTableQuery = query.ToCreateTableQuery("_" + info.Table, isTemporary: true);
        return instance;
    }

    public static T DefineTable<T>(Func<FluentSelectQuery<T>> builder) where T : ITableRowDefinition, new()
    {
        return DefineTable<T>(builder.Invoke());
    }

    public static T DefineTable<T>() where T : ITableRowDefinition, new()
    {
        var instance = new T();
        var clause = TableDefinitionClauseFactory.Create<T>();
        instance.CreateTableQuery = new CreateTableQuery(clause);
        return instance;
    }

    public static FluentSelectQuery From<T>(Expression<Func<T>> expression) where T : ITableRowDefinition
    {
        var sq = new FluentSelectQuery();

        var alias = ((MemberExpression)expression.Body).Member.Name;

        //execute
        var compiledExpression = expression.Compile();
        var result = compiledExpression();

        if (result.CreateTableQuery.Query != null)
        {
            sq.From(result.CreateTableQuery.Query).As(alias);
        }
        else if (result.CreateTableQuery.DefinitionClause != null)
        {
            sq.From(result.CreateTableQuery.DefinitionClause).As(alias);
        }

        return sq;
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

    public static string RowNumber() => string.Empty;

    public static string RowNumber(object partition, object order) => string.Empty;

    public static string RowNumberPartitionBy(object partition) => string.Empty;

    public static string RowNumberOrderBy(object order) => string.Empty;
}

public struct CTEDefinition
{
    public string Name { get; set; }

    public Type RowType { get; set; }

    public string Query { get; set; }
}
