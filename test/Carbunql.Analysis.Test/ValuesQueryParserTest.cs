using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class ValuesQueryParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public ValuesQueryParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void Default()
	{
		var text = @"
values
    (1,1),
    (2,2)
order by 1 desc 
limit 1";

		var sq = QueryParser.Parse(text);
		Monitor.Log(sq);

		Assert.Equal(17, sq.GetTokens().ToList().Count);

		var tables = sq.GetInternalQueries().SelectMany(x => x.GetSelectableTables());
		Assert.Empty(tables);
	}

	[Fact]
	public void ToSelectQuery()
	{
		var text = @"
values
    (1,1),
    (2,2)
union all
values
    (1,1),
    (2,2)
order by 1 desc 
limit 1";

		var q = QueryParser.Parse(text);

		var tables = q.GetInternalQueries().SelectMany(x => x.GetSelectableTables());
		Assert.Empty(tables);

		var sq = q.GetOrNewSelectQuery();

		Monitor.Log(sq);

		var expect = @"SELECT
    v.c0,
    v.c1
FROM
    (
        VALUES
            (1, 1),
            (2, 2)
        UNION ALL
        VALUES
            (1, 1),
            (2, 2)
        ORDER BY
            1 DESC
        LIMIT
            1
    ) AS v (
        c0, c1
    )".Replace("\r", "").Replace("\n", "");

		Assert.Equal(48, sq.GetTokens().ToList().Count);

		var tables2 = sq.GetInternalQueries().SelectMany(x => x.GetSelectableTables()).ToList();
		Assert.Single(tables2);
		Assert.Equal("", tables2[0].Table.GetTableFullName());
	}
}