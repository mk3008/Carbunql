using Carbunql.Analysis;
using Carbunql.Extensions;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class MergeTest
{
	private readonly QueryCommandMonitor Monitor;

	public MergeTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void MergeQuery()
	{
		var sql = "select a.id, a.sub_id, a.v1, a.v2 from table as a";
		var q = QueryParser.Parse(sql);

		var uq = q.ToMergeQuery("destination_table", new[] { "id", "sub_id" });
		Monitor.Log(uq);

		var lst = uq.GetTokens().ToList();

		Assert.Equal(90, lst.Count());
	}
}