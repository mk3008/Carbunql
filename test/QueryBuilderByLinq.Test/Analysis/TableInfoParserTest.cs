using Carbunql;
using QueryBuilderByLinq.Analysis;
using Xunit.Abstractions;
using static QueryBuilderByLinq.Sql;

namespace QueryBuilderByLinq.Test.Analysis;

public class TableInfoParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public TableInfoParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void DualTableTest()
	{
		var query = from a in Dual()
					select new
					{
						v1 = 1,
					};

		Monitor.Log(query);

		var from = TableInfoParser.Parse(query.Expression);

		Assert.Null(from);
	}

	[Fact]
	public void TypeTableTest()
	{
		var query = from a in FromTable<table_a>()
					select a;

		Monitor.Log(query);

		var from = TableInfoParser.Parse(query.Expression);

		Assert.Equal("table_a", from?.Table?.Alias);
		Assert.Equal("a", from?.Alias);
	}

	[Fact]
	public void StringTableTest()
	{
		var query = from a in FromTable<table_a>("TABLE_A")
					select a;

		Monitor.Log(query);

		var from = TableInfoParser.Parse(query.Expression);

		Assert.Equal("TABLE_A", from?.PhysicalName);
		Assert.Equal("a", from?.Alias);
	}

	[Fact]
	public void SubQueryTest()
	{
		var subquery = from a in FromTable<table_a>() select a.a_id;

		var query = from x in FromTable(subquery)
					select x;

		Monitor.Log(query);

		var from = TableInfoParser.Parse(query.Expression);

		Assert.Equal("select a.a_id from table_a as a", from?.Query?.ToOneLineText());
		Assert.Equal("x", from?.Alias);
	}

	[Fact]
	public void CommonTableTest()
	{
		var subquery = from a in FromTable<table_a>() select a.a_id;

		var query = from cte in CommonTable(subquery)
					from x in FromTable(cte)
					select x;

		Monitor.Log(query);

		var from = TableInfoParser.Parse(query.Expression);

		Assert.Equal("cte", from?.PhysicalName);
		Assert.Equal("x", from?.Alias);
	}

	[Fact]
	public void CommonTableNest2Test()
	{
		var subquery = from a in FromTable<table_a>() select a.a_id;

		var query = from cte1 in CommonTable(subquery)
					from cte2 in CommonTable(subquery)
					from x in FromTable(cte1)
					select x;

		Monitor.Log(query);

		var from = TableInfoParser.Parse(query.Expression);

		Assert.Equal("cte1", from?.PhysicalName);
		Assert.Equal("x", from?.Alias);
	}

	[Fact]
	public void CommonTableNestManyTest()
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

		var from = TableInfoParser.Parse(query.Expression);

		Assert.Equal("cte1", from?.PhysicalName);
		Assert.Equal("x", from?.Alias);
	}

	[Fact]
	public void CteAndDual()
	{
		var subquery = from a in FromTable<table_a>() select a.a_id;

		var query = from cte1 in CommonTable(subquery)
					from cte2 in CommonTable(subquery)
					from x in Dual()
					select x;

		Monitor.Log(query);

		var from = TableInfoParser.Parse(query.Expression);

		Assert.Null(from);
	}

	[Fact]
	public void CteAndTypeTable()
	{
		var subquery = from a in FromTable<table_a>() select a.a_id;

		var query = from cte1 in CommonTable(subquery)
					from cte2 in CommonTable(subquery)
					from x in FromTable<table_a>()
					select x;

		Monitor.Log(query);

		var from = TableInfoParser.Parse(query.Expression);

		Assert.Equal("table_a", from?.Table?.Alias);
		Assert.Equal("x", from?.Alias);
	}

	[Fact]
	public void CteAndStringTable()
	{
		var subquery = from a in FromTable<table_a>() select a.a_id;

		var query = from cte1 in CommonTable(subquery)
					from cte2 in CommonTable(subquery)
					from x in FromTable<table_a>("sales")
					select x;

		Monitor.Log(query);

		var from = TableInfoParser.Parse(query.Expression);

		Assert.Equal("sales", from?.PhysicalName);
		Assert.Equal("x", from?.Alias);
	}

	[Fact]
	public void CteAndSubQuery()
	{
		var subquery = from a in FromTable<table_a>() select a.a_id;

		var query = from cte1 in CommonTable(subquery)
					from x in FromTable(subquery)
					select x;

		Monitor.Log(query);

		var from = TableInfoParser.Parse(query.Expression);

		Assert.Equal("select a.a_id from table_a as a", from?.Query?.ToOneLineText());
		Assert.Equal("x", from?.Alias);
	}

	[Fact]
	public void JoinAndTypeTableTest()
	{
		var query = from s in FromTable<sale>()
					from a in InnerJoinTable<article>(x => s.article_id == x.article_id)
					select a;

		Monitor.Log(query);

		var from = TableInfoParser.Parse(query.Expression);
		Assert.Equal("sale", from?.ToSelectable().ToText());
		Assert.Equal("s", from?.Alias);
	}

	[Fact]
	public void JoinAndStringTableTest()
	{
		var query = from s in FromTable<sale>("sales")
					from a in InnerJoinTable<article>("articles", x => s.article_id == x.article_id)
					select a;

		Monitor.Log(query);

		var from = TableInfoParser.Parse(query.Expression);
		Assert.Equal("s", from?.Alias);
		Assert.Equal("sales AS s", from?.ToSelectable().ToText());
	}

	[Fact]
	public void JoinAndSubQueryTest()
	{
		var subquery = from sales in FromTable<sale>() select sales;

		var query = from s in FromTable(subquery)
					from a in InnerJoinTable<article>(x => s.article_id == x.article_id)
					select s;

		Monitor.Log(query);

		var from = TableInfoParser.Parse(query.Expression);

		Assert.Equal("select sales.sales_id, sales.article_id, sales.quantity from sale as sales", from?.Query?.ToOneLineText());
		Assert.Equal("s", from?.Alias);
	}

	public record struct table_a(int a_id, string text, int value);

	public record struct sale(int sales_id, int article_id, int quantity);

	public record struct article(int article_id, string article_name, int price);
}