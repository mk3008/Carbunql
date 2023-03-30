using Carbunql.Analysis;
using Carbunql.Values;
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
		uq.AddMatchedUpdate();
		uq.AddNotMathcedInsertAsAutoNumber();
		uq.AddNotMatchedInsert();

		Monitor.Log(uq);

		var lst = uq.GetTokens().ToList();

		Assert.Equal(119, lst.Count());
	}

	[Fact]
	public void MergeDeleteQuery()
	{
		var sql = "select a.tid, a.balance, a.deleted from (values (123, 10, true)) as a(tid, balance, deleted)";
		var q = QueryParser.Parse(sql);

		var uq = q.ToMergeQuery("target", new[] { "tid" });
		uq.AddMatchedDelete(() =>
		{
			return new ColumnValue(uq.DatasourceAlias, "deleted").True();
		});

		Monitor.Log(uq);

		var lst = uq.GetTokens().ToList();

		Assert.Equal(58, lst.Count());
	}
}