namespace Carbunql.Core;

public class QueryCommand
{
    public QueryCommand(string command, IDictionary<string, object?> prm)
    {
        CommandText = command;
        Parameters = prm;
    }

    public string CommandText { get; init; }

    public IDictionary<string, object?> Parameters { get; init; }
}