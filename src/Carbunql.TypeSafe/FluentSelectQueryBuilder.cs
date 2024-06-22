using System.Linq.Expressions;

namespace Carbunql.TypeSafe;

public class FluentSelectQueryBuilder(ISqlDialect sqlDialect)
{
    public readonly ISqlDialect SqlDialect = sqlDialect;

    public FluentSelectQuery<T> From<T>(Expression<Func<T>> expression) where T : IDataRow, new()
    {
        var sq = new FluentSelectQuery<T>(SqlDialect);

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
    public FluentSelectQuery From()
    {
        var sq = new FluentSelectQuery(SqlDialect);
        return sq;
    }
}
