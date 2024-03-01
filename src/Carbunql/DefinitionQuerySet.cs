namespace Carbunql;

public class DefinitionQuerySet
{
	public DefinitionQuerySet(CreateTableQuery createTableQuery)
	{
		CreateTableQuery = createTableQuery;
	}

	public CreateTableQuery CreateTableQuery { get; init; }
	public List<AlterTableQuery> AlterTableQueries { get; set; } = new();
	public List<CreateIndexQuery> CreateIndexQueries { get; set; } = new();
}
