namespace Carbunql;

public interface ITypeSafeDataSet
{
    /// <summary>
    /// Build the FROM clause
    /// </summary>
    /// <param name="query"></param>
    /// <param name="alias"></param>
    /// <returns></returns>
    SelectQuery BuildFromClause(SelectQuery query, string alias);

    /// <summary>
    /// Build the JOIN clause
    /// </summary>
    /// <param name="query"></param>
    /// <param name="join"></param>
    /// <param name="alias"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    SelectQuery BuildJoinClause(SelectQuery query, string join, string alias, string condition = "");
}
