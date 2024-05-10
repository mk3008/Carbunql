namespace Carbunql;

/// <summary>
/// Represents a query command with its command text and parameters.
/// </summary>
public class QueryCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryCommand"/> class with the specified command text and parameters.
    /// </summary>
    /// <param name="command">The command text.</param>
    /// <param name="prm">The parameters of the command.</param>
    public QueryCommand(string command, IEnumerable<QueryParameter> prm)
    {
        CommandText = command;
        Parameters = new Dictionary<string, object?>();
        foreach (var item in prm)
        {
            if (!Parameters.ContainsKey(item.ParameterName))
            {
                Parameters.Add(item.ParameterName, item.Value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the command text.
    /// </summary>
    public string CommandText { get; set; }

    /// <summary>
    /// Gets the parameters of the command.
    /// </summary>
    public Dictionary<string, object?> Parameters { get; init; }
}
