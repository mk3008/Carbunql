using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class OrderTest
{
	private readonly QueryCommandMonitor Monitor;

	public OrderTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void Default()
	{
		var sq = new SelectQuery();
		var (f, a) = sq.From("table_a").As("a");
		sq.Select(a, "column1").As("c1");
		sq.Select(a, "column2").As("c2");

		sq.GetSelectableItems().ToList().ForEach(x => sq.Group(x));
		sq.GetSelectableItems().ToList().ForEach(x => sq.Order(x));

		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();

		Assert.Equal(32, lst.Count());

		var sql = @"SELECT
    a.column1 AS c1,
    a.column2 AS c2
FROM
    table_a AS a
GROUP BY
    a.column1,
    a.column2
ORDER BY
    a.column1,
    a.column2";

		Assert.Equal(sql, sq.ToText(), true, true, true);
	}
}