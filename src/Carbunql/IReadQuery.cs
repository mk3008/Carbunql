namespace Carbunql;

public interface IReadQuery : IQueryCommandable
{
    ReadQuery GetQuery();
}