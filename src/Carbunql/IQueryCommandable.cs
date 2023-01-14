using Carbunql.Extensions;

namespace Carbunql;

public interface IQueryCommandable : IQueryCommand
{
    IDictionary<string, object?> GetParameters();
}

public static class IQueryCommandableExtension
{
    public static QueryCommand ToCommand(this IQueryCommandable source)
    {
        return new QueryCommand(source.GetTokens().ToText(), source.GetParameters());
    }

    public static QueryCommand ToCommand(this IQueryCommandable source, CommandTextBuilder builder)
    {
        return new QueryCommand(builder.Execute(source), source.GetParameters());
    }
}