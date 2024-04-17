using Carbunql.Clauses;
using Carbunql.Values;
using MessagePack;

namespace Carbunql;

[Union(0, typeof(SelectQuery))]
[Union(1, typeof(ValuesQuery))]
public interface IReadQuery : IQueryCommandable
{
    SelectClause? GetSelectClause();

    WithClause? GetWithClause();

    SelectQuery GetOrNewSelectQuery();

    IEnumerable<string> GetColumnNames();
}

public static class IReadQueryExtension
{
    public static ValueBase ToValue(this IReadQuery source)
    {
        return new QueryContainer(source);
    }

    public static IReadQuery DeepCopy(this IReadQuery source)
    {
        var json = MessagePackSerializer.Serialize(source);
        return MessagePackSerializer.Deserialize<IReadQuery>(json);
    }
}