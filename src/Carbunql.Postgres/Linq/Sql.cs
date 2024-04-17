﻿using Carbunql.Clauses;
using System.Linq.Expressions;

namespace Carbunql.Postgres.Linq;

public static class Sql
{
    /// <summary>
    /// This function returns a dual table.
    /// </summary>
    /// <returns>Returns an empty queryable object.</returns>
    public static IQueryable<DualTable> Dual()
    {
        return Enumerable.Empty<DualTable>().AsQueryable();
    }

    public static IQueryable<T> CommonTable<T>(IQueryable<T> subquery)
    {
        return new Table<T>(subquery);
    }

    public static IQueryable<T> CommonTable<T>(SelectQuery selectQuery)
    {
        return new Table<T>(selectQuery);
    }

    public static IQueryable<T> FromTable<T>(T rangeVariable)
    {
        return Enumerable.Empty<T>().AsQueryable();
    }

    public static IQueryable<T> FromTable<T>()
    {
        return Enumerable.Empty<T>().AsQueryable();
    }

    public static IQueryable<T> FromTable<T>(string tableName)
    {
        return new Table<T>(tableName);
    }

    public static IQueryable<T> FromTable<T>(IQueryable<T> subquery)
    {
        return new Table<T>(subquery);
    }

    public static IQueryable<T> FromTable<T>(SelectQuery selectQuery)
    {
        return new Table<T>(selectQuery);
    }

    public static IQueryable<T> InnerJoinTable<T>(Expression<Predicate<T>> condition)
    {
        return new Table<T>();
    }

    public static IQueryable<T> InnerJoinTable<T>(string tableName, Expression<Predicate<T>> condition)
    {
        return new Table<T>(tableName);
    }

    public static IQueryable<T> InnerJoinTable<T>(IQueryable<T> subquery, Expression<Predicate<T>> condition)
    {
        return new Table<T>(subquery);
    }

    public static IQueryable<T> InnerJoinTable<T>(T rangeVariable, Expression<Predicate<T>> condition)
    {
        return Enumerable.Empty<T>().AsQueryable();
    }

    public static IQueryable<T> InnerJoinTable<T>(SelectQuery selectQuery, Expression<Predicate<T>> condition)
    {
        return new Table<T>(selectQuery);
    }

    public static IQueryable<T> LeftJoinTable<T>(Expression<Predicate<T>> condition)
    {
        return new Table<T>();
    }

    public static IQueryable<T> LeftJoinTable<T>(string tableName, Expression<Predicate<T>> condition)
    {
        return new Table<T>(tableName);
    }

    public static IQueryable<T> LeftJoinTable<T>(IQueryable<T> subquery, Expression<Predicate<T>> condition)
    {
        return new Table<T>(subquery);
    }

    public static IQueryable<T> LeftJoinTable<T>(T rangeVariable, Expression<Predicate<T>> condition)
    {
        return Enumerable.Empty<T>().AsQueryable();
    }

    public static IQueryable<T> LeftJoinTable<T>(SelectQuery selectQuery, Expression<Predicate<T>> condition)
    {
        return new Table<T>(selectQuery);
    }

    public static IQueryable<T> CrossJoinTable<T>()
    {
        return Enumerable.Empty<T>().AsQueryable();
    }

    public static IQueryable<T> CrossJoinTable<T>(string tableName)
    {
        return new Table<T>(tableName);
    }

    public static IQueryable<T> CrossJoinTable<T>(IQueryable<T> subquery)
    {
        return new Table<T>(subquery);
    }

    public static IQueryable<T> CrossJoinTable<T>(T rangeVariable)
    {
        return Enumerable.Empty<T>().AsQueryable();
    }

    public static IQueryable<T> CrossJoinTable<T>(SelectQuery selectQuery)
    {
        return new Table<T>(selectQuery);
    }

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

    public static ValueBase Greatest(params object[] args)
    {
        throw new InvalidProgramException(ERROR);
    }

    public static ValueBase Least(params object[] args)
    {
        throw new InvalidProgramException(ERROR);
    }

    public static ValueBase RowNumber()
    {
        throw new InvalidProgramException(ERROR);
    }

    public static ValueBase RowNumber(object? orderby)
    {
        throw new InvalidProgramException(ERROR);
    }

    public static ValueBase RowNumber(object? partitionby, object? orderby)
    {
        throw new InvalidProgramException(ERROR);
    }
}
