using Carbunql.Analysis;
using Carbunql.Extensions;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class UpdateTest
{
	private readonly QueryCommandMonitor Monitor;

	public UpdateTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void UpdateQuery_Alias()
	{
		var sql = "select a.id, a.sub_id, a.v1, a.v2 from table as a";
		var q = QueryParser.Parse(sql);

		var uq = q.ToUpdateQuery("new_table".ToPhysicalTable().ToSelectable("t"), new[] { "id", "sub_id" });
		Monitor.Log(uq);

		var lst = uq.GetTokens().ToList();

		Assert.Equal(57, lst.Count());
	}

	[Fact]
	public void UpdateQuery()
	{
		var sql = "select a.id, a.sub_id, a.v1, a.v2 from table as a";
		var q = QueryParser.Parse(sql);

		var uq = q.ToUpdateQuery("new_table", new[] { "id", "sub_id" });
		Monitor.Log(uq);

		var lst = uq.GetTokens().ToList();

		Assert.Equal(57, lst.Count());
	}
}