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
		var subquery = from a in FromTable<table_a>() select a.a_id;

		var query = from cte in CommonTable(subquery)
					from x in FromTable(cte)
					select x;

		Monitor.Log(query);

		var ctes = CommonTableInfoParser.Parse(query.Expression);

		Assert.Single(ctes);
		Assert.Equal("cte", ctes[0].Alias);
	}

	[Fact]
	public void TwoCommonTablesTest()
	{
		var subquery = from a in FromTable<table_a>() select a.a_id;

		var query = from cte1 in CommonTable(subquery)
					from cte2 in CommonTable(subquery)
					from x in FromTable(cte1)
					select x;

		Monitor.Log(query);

		var ctes = CommonTableInfoParser.Parse(query.Expression);

		Assert.Equal(2, ctes.Count);
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

		var ctes = CommonTableInfoParser.Parse(query.Expression);

		Assert.Equal(5, ctes.Count);
	}

	public record struct table_a(int a_id, string text, int value);
}