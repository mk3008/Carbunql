using Carbunql.Clauses;

namespace Carbunql;

public interface IReadQuery : IQueryCommandable
{
    SelectClause? GetSelectClause();

    ReadQuery GetQuery();
}