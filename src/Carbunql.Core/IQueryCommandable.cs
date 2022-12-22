namespace Carbunql.Core;

public interface IQueryCommandable : IQueryCommand, IQueryParameter
{
    QueryCommand ToCommand();
}