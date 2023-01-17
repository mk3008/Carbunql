using Carbunql.Analysis;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class CreateTableTest
{
	private readonly QueryCommandMonitor Monitor;

	public CreateTableTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void CreateTable()
	{
		var sql = "select a.id, a.value from table as a";
		var q = QueryParser.Parse(sql);

		var ctq = q.ToCreateTableQuery("new_table");
		ctq.CreateTableClause.IsTemporary = false;

		Monitor.Log(ctq);

		var lst = ctq.GetTokens().ToList();

		Assert.Equal(15, lst.Count());
	}

	[Fact]
	public void CreateTemporaryTable()
	{
		var sql = "select a.id, a.value from table as a";
		var q = QueryParser.Parse(sql);

		var ctq = q.ToCreateTableQuery("new_table");
		Monitor.Log(ctq);

		var lst = ctq.GetTokens().ToList();

		Assert.Equal(15, lst.Count());
	}
}