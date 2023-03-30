using Carbunql.Analysis;
using Carbunql.Extensions;
using Carbunql.Values;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class InsertTest
{
	private readonly QueryCommandMonitor Monitor;

	public InsertTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void InsertQuery()
	{
		var sql = "select a.id, a.value as v from table as a";
		var q = QueryParser.Parse(sql);

		var iq = q.ToInsertQuery("new_table");
		Monitor.Log(iq);

		var lst = iq.GetTokens().ToList();

		Assert.Equal(21, lst.Count());
	}

	[Fact]
	public void InsertQuery_Values()
	{
		var sql = "values (1, 'a'), (2, 'b')";
		var q = QueryParser.Parse(sql);

		var iq = q.ToInsertQuery("new_table");
		Monitor.Log(iq);

		var lst = iq.GetTokens().ToList();

		Assert.Equal(14, lst.Count());
	}

	[Fact]
	public void InsertQuery_ColumnFilter()
	{
		var sql = "select a.id, a.value as v from table as a";
		var tmp = QueryParser.Parse(sql);

		var sq = new SelectQuery();
		var (f, q) = sq.From(tmp).As("q");
		q.GetColumnNames().Where(x => x.IsEqualNoCase("id")).ToList().ForEach(x => sq.Select(q, x));

		var iq = sq.ToInsertQuery("new_table");
		Monitor.Log(iq);

		var lst = iq.GetTokens().ToList();

		Assert.Equal(28, lst.Count());
	}

	[Fact]
	public void InsertQuery_Returning()
	{
		var sql = "select a.id, a.value as v from table as a";
		var tmp = QueryParser.Parse(sql);

		var sq = new SelectQuery();
		var (f, q) = sq.From(tmp).As("q");
		q.GetColumnNames().Where(x => x.IsEqualNoCase("id")).ToList().ForEach(x => sq.Select(q, x));

		var iq = sq.ToInsertQuery("new_table");
		iq.Returning("seq");
		Monitor.Log(iq);

		var lst = iq.GetTokens().ToList();

		Assert.Equal(30, lst.Count());
	}
}