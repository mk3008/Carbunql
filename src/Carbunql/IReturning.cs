using Carbunql.Clauses;
using Carbunql.Values;

namespace Carbunql;

/// <summary>
/// Represents an interface for queries that support RETURNING clauses.
/// </summary>
public interface IReturning
{
    /// <summary>
    /// Gets or sets the RETURNING clause of the query.
    /// </summary>
    ReturningClause? ReturningClause { get; set; }
}

/// <summary>
/// Provides extension methods for objects implementing the <see cref="IReturning"/> interface.
/// </summary>
public static class IReturningExtension
{
    /// <summary>
    /// Adds a returning clause to the query with the specified value.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="IReturning"/> interface.</param>
    /// <param name="value">The value to be returned.</param>
    /// <exception cref="Exception">Thrown when the returning clause is not empty.</exception>
    public static void Returning(this IReturning source, ValueBase value)
    {
        if (source.ReturningClause != null) throw new Exception("returning clause is not empty.");
        source.ReturningClause = new ReturningClause(value);
    }

    /// <summary>
    /// Adds a returning clause to the query with the value obtained from the specified builder function.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="IReturning"/> interface.</param>
    /// <param name="builder">A function that returns the value to be returned.</param>
    public static void Returning(this IReturning source, Func<ValueBase> builder)
    {
        source.Returning(builder());
    }

    /// <summary>
    /// Adds a returning clause to the query with the specified column name.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="IReturning"/> interface.</param>
    /// <param name="columnName">The name of the column to be returned.</param>
    public static void Returning(this IReturning source, string columnName)
    {
        source.Returning(new ColumnValue(columnName));
    }

    /// <summary>
    /// Adds a returning clause to the query returning all columns.
    /// </summary>
    /// <param name="source">The object implementing the <see cref="IReturning"/> interface.</param>
    public static void ReturningAll(this IReturning source)
    {
        source.Returning("*");
    }
}
