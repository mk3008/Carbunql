namespace Carbunql.TypeSafe;

public interface IDatasource
{
    bool IsCTE { get; }

    SelectQuery BuildFromClause(SelectQuery query, string alias);

    SelectQuery BuildJoinClause(SelectQuery query, string join, string alias, string condition = "");
}
