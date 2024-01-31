namespace Carbunql;

public class QueryCommand
{
	public QueryCommand(string command, IEnumerable<QueryParameter> prm)
	{
		CommandText = command;
		Parameters = prm;
	}

	public string CommandText { get; set; }

	public IEnumerable<QueryParameter> Parameters { get; init; }
}