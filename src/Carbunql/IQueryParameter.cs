namespace Carbunql;

public interface IQueryParameter : IQueryCommand
{
	IDictionary<string, object?> GetParameters();
}