namespace Carbunql;

/// <summary>
/// Represents a SQL query command with its command text and parameters.
/// </summary>
/// <remarks>
/// This class encapsulates a SQL query command, consisting of the command text representing the SQL statement
/// and a collection of parameters associated with the command. Parameters are placeholders in the SQL statement
/// that are replaced with specific values during execution.
/// </remarks>
public class QueryCommand
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryCommand"/> class with the specified command text and parameters.
    /// </summary>
    /// <param name="command">The command text representing the SQL statement.</param>
    /// <param name="prm">The parameters associated with the command.</param>
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
    /// Gets or sets the command text of the query.
    /// The command text represents the SQL statement, such as a SELECT statement or an INSERT statement.
    /// </summary>
    public string CommandText { get; set; }

    /// <summary>
    /// Gets the parameters associated with the command.
    /// </summary>
    /// <remarks>
    /// The parameters represent placeholders in the SQL command that are replaced with specific values during execution.
    /// </remarks>
    public Dictionary<string, object?> Parameters { get; init; }
}
