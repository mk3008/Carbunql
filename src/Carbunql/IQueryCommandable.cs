namespace Carbunql;

public interface IQueryCommandable : IQueryCommand, IQueryParameter
{
	QueryCommand ToCommand();
}