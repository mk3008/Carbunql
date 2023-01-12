namespace Carbunql;

public class QueryCommand
{
    public QueryCommand(string command, IDictionary<string, object?> prm)
    {
        CommandText = command;
        Parameters = prm;
    }

    public string CommandText { get; set; }

    public IDictionary<string, object?> Parameters { get; init; }
}