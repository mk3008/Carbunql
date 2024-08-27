using Carbunql.TypeSafe.Dialect;
using System.Linq.Expressions;

namespace Carbunql.TypeSafe;

/// <summary>
/// A select query builder that supports SQL dialects
/// </summary>
/// <param name="sqlDialect">SQL dialect interface</param>
public class FluentSelectQueryBuilder(ISqlTranspiler sqlDialect)
{
    /// <summary>
    /// The SQL dialect used by the query builder.
    /// </summary>
    public readonly ISqlTranspiler SqlDialect = sqlDialect;

    /// <summary>
    /// Creates a select query with a From clause.
    /// </summary>
    /// <typeparam name="T">The type of the data row.</typeparam>
    /// <param name="expression">An expression to specify the data row.</param>
    /// <returns>A FluentSelectQuery object for the specified data row type.</returns>
    public FluentSelectQuery<T> From<T>(Expression<Func<T>> expression) where T : IDataRow, new()
    {
        var sq = new FluentSelectQuery<T>(SqlDialect);

        var alias = ((MemberExpression)expression.Body).Member.Name;

        // Execute the expression to get the result.
        var compiledExpression = expression.Compile();
        var result = compiledExpression();

        // Build the FROM clause using the result's DataSet and alias.
        result.DataSet.BuildFromClause(sq, alias);

        return sq;
    }

    /// <summary>
    /// Creates a select query without a From clause.
    /// </summary>
    /// <returns>A FluentSelectQuery object.</returns>
    public FluentSelectQuery From()
    {
        var sq = new FluentSelectQuery(SqlDialect);
        return sq;
    }
}
