using Carbunql.Clauses;
using Carbunql.Values;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class WindowTest
{
	private readonly QueryCommandMonitor Monitor;

	public WindowTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void Default()
	{
		var sq = new SelectQuery();
		var (_, a) = sq.From("table_a").As("a");

		var pc = new PartitionClause
		{
			new ColumnValue(a, "name")
		};
		var oc = new OrderClause {
			new ColumnValue(a, "a_id").ToSortable()
		};
		var wd = new WindowDefinition(pc, oc);
		var w1 = new NamedWindowDefinition("w1", wd);

		sq.WindowClause ??= new();
		sq.WindowClause.Add(w1);

		sq.Select(new FunctionValue("row_number", new OverClause(w1))).As("row_num");

		Monitor.Log(sq);

		var expect = @"SELECT
    ROW_NUMBER() OVER w1 AS row_num
FROM
    table_a AS a
WINDOW
    w1 AS (
        PARTITION BY
            a.name
        ORDER BY
            a.a_id
    )";

		Assert.Equal(expect, sq.ToText().ToString(), true, true, true);
	}

	[Fact]
	public void NamedWindow()
	{
		var sq = new SelectQuery();
		var (_, a) = sq.From("table_a").As("a");

		var w = new NamedWindowDefinition("w1");
		w.AddPartition(new ColumnValue(a, "name"));
		w.AddOrder(new ColumnValue(a, "a_id"));

		sq.Window(w);

		sq.Select(new FunctionValue("row_number", new OverClause(w))).As("row_num");

		Monitor.Log(sq);

		var expect = @"SELECT
    ROW_NUMBER() OVER w1 AS row_num
FROM
    table_a AS a
WINDOW
    w1 AS (
        PARTITION BY
            a.name
        ORDER BY
            a.a_id
    )";

		Assert.Equal(expect, sq.ToText().ToString(), true, true, true);
	}
}