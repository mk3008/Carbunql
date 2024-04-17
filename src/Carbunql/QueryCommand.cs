namespace Carbunql;

public class QueryCommand
{
    public QueryCommand(string command, IEnumerable<QueryParameter> prm)
    {
        CommandText = command;
        Parameters = new Dictionary<string, object?>();
        foreach (var item in prm)
        {
            if (Parameters.ContainsKey(item.ParameterName)) continue;
            Parameters.Add(item.ParameterName, item.Value);
        }
    }

    public string CommandText { get; set; }

    public Dictionary<string, object?> Parameters { get; init; }
}