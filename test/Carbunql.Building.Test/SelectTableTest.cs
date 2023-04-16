using Carbunql.Clauses;
using Carbunql.Values;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class SelectTableTest
{
	private readonly QueryCommandMonitor Monitor;

	public SelectTableTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void SubQuery()
	{
		var sq = new SelectQuery();
		var (f, a) = sq.From(() => new SelectQuery("select x.v1, v2, v3 from table_a")).As("a");
		sq.Select(a);

		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(27, lst.Count());
	}
}
