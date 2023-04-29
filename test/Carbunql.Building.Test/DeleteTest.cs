using Carbunql.Analysis;
using Carbunql.Extensions;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class DeleteTest
{
	private readonly QueryCommandMonitor Monitor;

	public DeleteTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void DeleteQuery_Alias()
	{
		var sql = "select a.id, a.sub_id, a.v1, a.v2 from table as a";
		var q = QueryParser.Parse(sql);

		var uq = q.ToDeleteQuery("new_table".ToPhysicalTable().ToSelectable("t"), new[] { "id", "sub_id" });
		Monitor.Log(uq);

		var lst = uq.GetTokens().ToList();

		Assert.Equal(50, lst.Count());
	}

	[Fact]
	public void DeleteQuery()
	{
		var sql = "select a.id, a.sub_id, a.v1, a.v2 from table as a";
		var q = QueryParser.Parse(sql);

		var uq = q.ToDeleteQuery("new_table", new[] { "id", "sub_id" });
		Monitor.Log(uq);

		var lst = uq.GetTokens().ToList();

		Assert.Equal(50, lst.Count());
	}

	[Fact]
	public void DeleteQueryArgumentOmitted()
	{
		var sql = "select a.id, a.sub_id, a.v1, a.v2 from table as a";
		var q = QueryParser.Parse(sql);

		var uq = q.ToDeleteQuery("new_table");
		Monitor.Log(uq);

		var lst = uq.GetTokens().ToList();

		Assert.Equal(45, lst.Count());
	}
}