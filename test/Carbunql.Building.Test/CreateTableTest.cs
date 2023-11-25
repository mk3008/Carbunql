using Carbunql.Analysis;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class CreateTableTest
{
	private readonly QueryCommandMonitor sq;

	public CreateTableTest(ITestOutputHelper output)
	{
		sq = new QueryCommandMonitor(output);
	}

	[Fact]
	public void CreateTable()
	{
		var sql = "select a.id, a.value from table as a";
		var q = QueryParser.Parse(sql);

		var ctq = q.ToCreateTableQuery("new_table");
		ctq.CreateTableClause.IsTemporary = false;

		sq.Log(ctq);

		var lst = ctq.GetTokens().ToList();

		Assert.Equal(15, lst.Count());

		var expect = @"
CREATE TABLE
    new_table
AS
SELECT
    a.id,
    a.value
FROM
    table AS a";

		Assert.Equal(expect.ToValidateText(), ctq.ToText().ToValidateText());
	}

	[Fact]
	public void CreateTemporaryTable()
	{
		var sql = "select a.id, a.value from table as a";
		var q = QueryParser.Parse(sql);

		var ctq = q.ToCreateTableQuery("new_table");
		sq.Log(ctq);

		var lst = ctq.GetTokens().ToList();

		Assert.Equal(15, lst.Count());
	}

	[Fact]
	public void SelectQuery()
	{
		var sql = "select a.id, 'test' as value from table as a";
		var q = QueryParser.Parse(sql);

		var sq = q.ToCreateTableQuery("new_table").ToSelectQuery();
		this.sq.Log(sq);

		var lst = sq.GetTokens().ToList();

		Assert.Equal(12, lst.Count());

		var expect = @"
SELECT
    t.id,
    t.value
FROM
    new_table AS t";

		Assert.Equal(expect.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void CountQuery()
	{
		var sql = "select a.id, 'test' as value from table as a";
		var q = QueryParser.Parse(sql);

		var sq = q.ToCreateTableQuery("new_table").ToCountQuery();
		this.sq.Log(sq);

		var lst = sq.GetTokens().ToList();

		Assert.Equal(11, lst.Count());

		var expect = @"
SELECT
    COUNT(*) AS row_count
FROM
    new_table AS q";

		Assert.Equal(expect.ToValidateText(), sq.ToText().ToValidateText());
	}
}