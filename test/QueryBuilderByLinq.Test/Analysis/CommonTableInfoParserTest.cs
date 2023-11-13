using Carbunql;
using QueryBuilderByLinq.Analysis;
using Xunit.Abstractions;
using static QueryBuilderByLinq.Sql;

namespace QueryBuilderByLinq.Test.Analysis;

public class CommonTableInfoParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public CommonTableInfoParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void SingleCommonTableTest()
	{
		var subquery = from a in FromTable<table_a>() select new { a.a_id };

		var query = from cte in CommonTable(subquery)
					from x in FromTable(cte)
					select x;

		Monitor.Log(query);

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var ctes = CommonTableInfoParser.Parse(query.Expression);

		Assert.Single(ctes);
		Assert.Equal("cte", ctes[0].Alias);
		Assert.Equal("select a.a_id from table_a as a", ctes[0].Query.ToSelectQuery().ToOneLineText());

		var sql = @"
WITH
    cte AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    )
SELECT
    x.a_id
FROM
    cte AS x";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}

	[Fact]
	public void TwoCommonTablesTest()
	{
		var sub1 = from a in FromTable<table_a>() select a.a_id;
		var sub2 = from b in FromTable<table_a>() select b.text;

		var query = from cte1 in CommonTable(sub1)
					from cte2 in CommonTable(sub2)
					from x in FromTable(cte1)
					select x;

		Monitor.Log(query);

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var ctes = CommonTableInfoParser.Parse(query.Expression);

		Assert.Equal(2, ctes.Count);
		Assert.Equal("cte1", ctes[0].Alias);
		Assert.Equal("select a.a_id from table_a as a", ctes[0].Query.ToSelectQuery().ToOneLineText());
		Assert.Equal("cte2", ctes[1].Alias);
		Assert.Equal("select b.text from table_a as b", ctes[1].Query.ToSelectQuery().ToOneLineText());

		//parse not working 
		//  join
		//  select item

		var sql = @"
WITH
    cte AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    )
SELECT
    x.a_id
FROM
    cte AS x";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}

	[Fact]
	public void ManyCommonTablesTest()
	{
		var subquery = from a in FromTable<table_a>() select a.a_id;

		var query = from cte1 in CommonTable(subquery)
					from cte2 in CommonTable(subquery)
					from cte3 in CommonTable(subquery)
					from cte4 in CommonTable(subquery)
					from cte5 in CommonTable(subquery)
					from x in FromTable(cte1)
					select x;

		Monitor.Log(query);

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var ctes = CommonTableInfoParser.Parse(query.Expression);

		Assert.Equal(5, ctes.Count);
		Assert.Equal("cte1", ctes[0].Alias);
		Assert.Equal("cte2", ctes[1].Alias);
		Assert.Equal("cte3", ctes[2].Alias);
		Assert.Equal("cte4", ctes[3].Alias);
		Assert.Equal("cte5", ctes[4].Alias);

		var sql = @"
WITH
    cte AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    )
SELECT
    x.a_id
FROM
    cte AS x";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}

	public record struct table_a(int a_id, string text, int value);
}