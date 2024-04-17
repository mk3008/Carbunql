﻿using Dapper;
using System.Data;
using static Dapper.SqlMapper;

namespace Carbunql.Dapper;

public static class IDbConnectionExtension
{
    private static QueryCommand GetQueryCommand(IQueryCommandable q)
    {
        if (Formatter.Type == FormatType.Simple)
        {
            return q.ToOneLineCommand();
        }
        return q.ToCommand();
    }

    public static int Execute(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.Execute(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static int Execute(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.Execute(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static object ExecuteScalar(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.ExecuteScalar(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static object ExecuteScalar(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.ExecuteScalar(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static T ExecuteScalar<T>(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.ExecuteScalar<T>(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static T ExecuteScalar<T>(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.ExecuteScalar<T>(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static IDataReader ExecuteReader(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.ExecuteReader(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static IDataReader ExecuteReader(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.ExecuteReader(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static IEnumerable<dynamic> Query(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.Query(GetQueryCommand(q), transaction, buffered, commandTimeout, commandType);
    }
    public static IEnumerable<dynamic> Query(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.Query(q.CommandText, q.Parameters, transaction, buffered, commandTimeout, commandType);
    }
    public static dynamic QueryFirst(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryFirst(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static dynamic QueryFirst(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryFirst(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static dynamic QueryFirstOrDefault(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryFirstOrDefault(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static dynamic QueryFirstOrDefault(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryFirstOrDefault(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static dynamic QuerySingle(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QuerySingle(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static dynamic QuerySingle(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QuerySingle(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static dynamic QuerySingleOrDefault(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QuerySingleOrDefault(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static dynamic QuerySingleOrDefault(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QuerySingleOrDefault(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static IEnumerable<T> Query<T>(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.Query<T>(GetQueryCommand(q), transaction, buffered, commandTimeout, commandType);
    }
    public static IEnumerable<T> Query<T>(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.Query<T>(q.CommandText, q.Parameters, transaction, buffered, commandTimeout, commandType);
    }
    public static T QueryFirst<T>(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryFirst<T>(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static T QueryFirst<T>(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryFirst<T>(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static T QueryFirstOrDefault<T>(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryFirstOrDefault<T>(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static T QueryFirstOrDefault<T>(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryFirstOrDefault<T>(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static T QuerySingle<T>(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QuerySingle<T>(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static T QuerySingle<T>(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QuerySingle<T>(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static T QuerySingleOrDefault<T>(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QuerySingleOrDefault<T>(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static T QuerySingleOrDefault<T>(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QuerySingleOrDefault<T>(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static IEnumerable<object> Query(this IDbConnection cnn, Type type, IQueryCommandable q, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.Query(GetQueryCommand(q), transaction, buffered, commandTimeout, commandType);
    }
    public static IEnumerable<object> Query(this IDbConnection cnn, Type type, QueryCommand q, IDbTransaction? transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.Query(q.CommandText, q.Parameters, transaction, buffered, commandTimeout, commandType);
    }
    public static object QueryFirst(this IDbConnection cnn, Type type, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryFirst(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static object QueryFirst(this IDbConnection cnn, Type type, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryFirst(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static object QueryFirstOrDefault(this IDbConnection cnn, Type type, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryFirstOrDefault(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static object QueryFirstOrDefault(this IDbConnection cnn, Type type, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryFirstOrDefault(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static object QuerySingle(this IDbConnection cnn, Type type, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QuerySingle(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static object QuerySingle(this IDbConnection cnn, Type type, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QuerySingle(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static object QuerySingleOrDefault(this IDbConnection cnn, Type type, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QuerySingleOrDefault(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static object QuerySingleOrDefault(this IDbConnection cnn, Type type, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QuerySingleOrDefault(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
    public static GridReader QueryMultiple(this IDbConnection cnn, IQueryCommandable q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryMultiple(GetQueryCommand(q), transaction, commandTimeout, commandType);
    }
    public static GridReader QueryMultiple(this IDbConnection cnn, QueryCommand q, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return cnn.QueryMultiple(q.CommandText, q.Parameters, transaction, commandTimeout, commandType);
    }
}