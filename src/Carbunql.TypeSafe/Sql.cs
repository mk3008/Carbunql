﻿using Carbunql.Annotations;
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
        else if (expression.Body is InvocationExpression ie)
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

    public static T DefineDataSet<T>() where T : IDataRow, new()
    {
        var atr = (TableAttribute?)Attribute.GetCustomAttribute(typeof(T), typeof(TableAttribute));

        var schema = string.IsNullOrEmpty(atr?.Schema) ? string.Empty : atr.Schema;
        var table = string.IsNullOrEmpty(atr?.Table) ? typeof(T).Name : atr.Table;
        var info = new TableInfo(table) { Schema = schema };

        var columnNames = PropertySelector.SelectLiteralProperties<T>().Select(x => x.Name);

        var instance = new T();
        instance.DataSet = new PhysicalTableDataSet(info, columnNames);
        return instance;
    }

    public static FluentSelectQuery<T> From<T>(Expression<Func<T>> expression) where T : IDataRow, new()
    {
        var sq = new FluentSelectQuery<T>();

        var alias = ((MemberExpression)expression.Body).Member.Name;

        //execute
        var compiledExpression = expression.Compile();
        var result = compiledExpression();

        result.DataSet.BuildFromClause(sq, alias);

        return sq;
    }


    /// <summary>
    /// Select query without a From clause
    /// </summary>
    /// <returns></returns>
    public static FluentSelectQuery From()
    {
        var sq = new FluentSelectQuery();
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

    [Obsolete("user DateTime.Now")]
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
