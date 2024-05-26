using Carbunql.Annotations;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe;

/// <summary>
/// Only function definitions are written for use in expression trees.
/// The actual situation is in ExpressionExtension.cs.
/// </summary
public static class Sql
{
    private static T DefineCTE<T>(Expression<Func<FluentSelectQuery<T>>> expression, Materialized materialization) where T : ITableRowDefinition, new()
    {
#if DEBUG
        var analyze = ExpressionReader.Analyze(expression);
        // Debug analysis for the expression
#endif
        if (expression.Body is MemberExpression body)
        {
            // Compile and execute the expression
            var compiledExpression = expression.Compile();
            var result = compiledExpression();

            if (result is SelectQuery sq)
            {
                var variableName = body.Member.Name;

                var instance = new T();
                instance.Datasource = new CTEDatasoure(variableName, sq)
                {
                    Materialized = materialization
                };

                return instance;
            }
            else
            {
                throw new NotSupportedException("The provided expression did not result in a SelectQuery.");
            }
        }
        throw new NotSupportedException("Expression body is not a MemberExpression.");
    }


    public static T DefineCTE<T>(Expression<Func<FluentSelectQuery<T>>> expression) where T : ITableRowDefinition, new()
    {
        return DefineCTE(expression, Materialized.Undefined);
    }

    public static T DefineMaterializedCTE<T>(Expression<Func<FluentSelectQuery<T>>> expression) where T : ITableRowDefinition, new()
    {
        return DefineCTE(expression, Materialized.Materialized);
    }

    public static T DefineNotMaterializedCTE<T>(Expression<Func<FluentSelectQuery<T>>> expression) where T : ITableRowDefinition, new()
    {
        return DefineCTE(expression, Materialized.NotMaterialized);
    }

    public static T DefineSubQuery<T>(SelectQuery query) where T : ITableRowDefinition, new()
    {
        var instance = new T();

        //var info = TableInfoFactory.Create(typeof(T));

        instance.Datasource = new QueryDatasource(query);
        return instance;
    }

    public static T DefineSubQuery<T>(FluentSelectQuery<T> query) where T : ITableRowDefinition, new()
    {
        var instance = new T();

        //var info = TableInfoFactory.Create(typeof(T));

        instance.Datasource = new QueryDatasource(query);
        return instance;
    }

    public static T DefineSubQuery<T>(Func<FluentSelectQuery<T>> builder) where T : ITableRowDefinition, new()
    {
        return DefineSubQuery<T>(builder.Invoke());
    }

    public static T DefineTable<T>() where T : ITableRowDefinition, new()
    {
        var instance = new T();
        var clause = TableDefinitionClauseFactory.Create<T>();
        instance.Datasource = new PhysicalTableDatasource(clause);
        return instance;
    }

    public static FluentSelectQuery From<T>(Expression<Func<T>> expression) where T : ITableRowDefinition
    {
        var sq = new FluentSelectQuery();

        var alias = ((MemberExpression)expression.Body).Member.Name;

        //execute
        var compiledExpression = expression.Compile();
        var result = compiledExpression();

        result.Datasource.BuildFromClause(sq, alias);

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
