namespace Carbunql.TypeSafe;

public interface IDataSet
{
    public List<string> Columns { get; }

    SelectQuery BuildFromClause(SelectQuery query, string alias);

    SelectQuery BuildJoinClause(SelectQuery query, string join, string alias, string condition = "");
}
