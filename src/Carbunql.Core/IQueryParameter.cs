namespace Carbunql.Core;

public interface IQueryParameter : IQueryCommand
{
    IDictionary<string, object?> GetParameters();
}