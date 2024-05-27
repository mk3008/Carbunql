﻿using Carbunql.Annotations;
using System.ComponentModel.Design;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe;

/// <summary>
/// Only function definitions are written for use in expression trees.
/// The actual situation is in ExpressionExtension.cs.
/// </summary
public static class Sql
{
    private static T DefineDataSet<T>(Expression<Func<FluentSelectQuery<T>>> expression, Materialized materialization) where T : IDataRow, new()
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
                instance.DataSet = new CTEDataSet(variableName, sq)
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
        else if (expression.Body is MethodCallExpression me)
        {
            var compiledExpression = expression.Compile();
            var result = compiledExpression();
            if (result is SelectQuery sq)
            {
                var instance = new T();
                instance.DataSet = new QueryDataSet(sq);
                return instance;
            }
            else
            {
                throw new NotSupportedException("The provided expression did not result in a SelectQuery.");
            }
        }
        throw new NotSupportedException("Expression body is not a MemberExpression.");
    }

    public static T2 DefineDataSet<T1, T2>(Expression<Func<FluentSelectQuery<T1>>> expression, Func<FluentSelectQuery<T1>, FluentSelectQuery<T2>> editor) where T1 : IDataRow, new() where T2 : IDataRow, new()
    {
#if DEBUG
        var analyze = ExpressionReader.Analyze(expression);
        // Debug analysis for the expression
#endif
        var compiledExpression = expression.Compile();
        var result = compiledExpression();
        if (result is FluentSelectQuery<T1> sq)
        {
            var edited = editor(sq);

            var instance = new T2();

            instance.DataSet = new QueryDataSet(edited);
            return instance;
        }
        else
        {
            throw new NotSupportedException("The provided expression did not result in a SelectQuery.");
        }
    }

    public static T DefineDataSet<T>(Expression<Func<FluentSelectQuery<T>>> expression) where T : IDataRow, new()
    {
        return DefineDataSet(expression, Materialized.Undefined);
    }

    public static T DefineMaterializedDataSet<T>(Expression<Func<FluentSelectQuery<T>>> expression) where T : IDataRow, new()
    {
        return DefineDataSet(expression, Materialized.Materialized);
    }

    public static T DefineNotMaterializedDataSet<T>(Expression<Func<FluentSelectQuery<T>>> expression) where T : IDataRow, new()
    {
        return DefineDataSet(expression, Materialized.NotMaterialized);
    }

    public static T DefineSubQuery<T>(SelectQuery query) where T : IDataRow, new()
    {
        var instance = new T();

        //var info = TableInfoFactory.Create(typeof(T));

        instance.DataSet = new QueryDataSet(query);
        return instance;
    }

    public static T DefineSubQuery<T>(FluentSelectQuery<T> query) where T : IDataRow, new()
    {
        var instance = new T();

        instance.DataSet = new QueryDataSet(query);
        return instance;
    }

    public static T DefineSubQuery<T>(Func<FluentSelectQuery<T>> builder) where T : IDataRow, new()
    {
        return DefineSubQuery<T>(builder.Invoke());
    }

    public static T DefineDataSet<T>() where T : IDataRow, new()
    {
        var instance = new T();
        var clause = TableDefinitionClauseFactory.Create<T>();
        instance.DataSet = new PhysicalTableDataSet(clause, clause.GetColumnNames());
        return instance;
    }

    public static FluentSelectQuery From<T>(Expression<Func<T>> expression) where T : IDataRow
    {
        var sq = new FluentSelectQuery();

        var alias = ((MemberExpression)expression.Body).Member.Name;

        //execute
        var compiledExpression = expression.Compile();
        var result = compiledExpression();

        result.DataSet.BuildFromClause(sq, alias);

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
