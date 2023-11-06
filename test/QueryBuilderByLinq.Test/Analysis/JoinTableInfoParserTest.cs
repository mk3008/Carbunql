using Carbunql;
using QueryBuilderByLinq.Analysis;
using Xunit.Abstractions;
using static QueryBuilderByLinq.Sql;

namespace QueryBuilderByLinq.Test.Analysis;

public class JoinTableInfoParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public JoinTableInfoParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void NoRelationTest()
	{
		var query = from a in FromTable<sale>()
					select a;

		Monitor.Log(query);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Empty(joins);
	}

	[Fact]
	public void InnerJoinTypeTableTest()
	{
		var query = from s in FromTable<sale>()
					from a in InnerJoinTable<article>(x => s.article_id == x.article_id)
					select a;

		Monitor.Log(query);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Single(joins);

		Assert.Equal("article", joins[0].TableInfo.ToSelectable().ToOneLineText());
		Assert.Equal("InnerJoinTable", joins[0].Relation);
		Assert.Equal("s.article_id = a.article_id", joins[0].Condition!.ToOneLineText());
	}

	[Fact]
	public void InnerJoinStringTableTest()
	{
		var query = from s in FromTable<sale>("sales")
					from a in InnerJoinTable<article>("articles", x => s.article_id == x.article_id)
					select a;

		Monitor.Log(query);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Single(joins);

		Assert.Equal("articles as a", joins[0].TableInfo.ToSelectable().ToOneLineText());
		Assert.Equal("InnerJoinTable", joins[0].Relation);
		Assert.Equal("s.article_id = a.article_id", joins[0].Condition!.ToOneLineText());
	}

	[Fact]
	public void LeftJoinTest()
	{
		var query = from s in FromTable<sale>()
					from a in LeftJoinTable<article>(x => s.article_id == x.article_id)
					select a;

		Monitor.Log(query);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Equal("article", joins[0].TableInfo.ToSelectable().ToOneLineText());
		Assert.Equal("LeftJoinTable", joins[0].Relation);
		Assert.Equal("s.article_id = a.article_id", joins[0].Condition!.ToOneLineText());
	}

	[Fact]
	public void LeftJoinStringTableTest()
	{
		var query = from s in FromTable<sale>("sales")
					from a in LeftJoinTable<article>("articles", x => s.article_id == x.article_id)
					select a;

		Monitor.Log(query);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Single(joins);

		Assert.Equal("articles as a", joins[0].TableInfo.ToSelectable().ToOneLineText());
		Assert.Equal("LeftJoinTable", joins[0].Relation);
		Assert.Equal("s.article_id = a.article_id", joins[0].Condition!.ToOneLineText());
	}

	[Fact]
	public void CrossJoinTest()
	{
		var query = from s in FromTable<sale>()
					from a in CrossJoinTable<article>()
					select a;

		Monitor.Log(query);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Equal("article", joins[0].TableInfo.ToSelectable().ToOneLineText());
		Assert.Equal("CrossJoinTable", joins[0].Relation);
		Assert.Null(joins[0].Condition);
	}

	[Fact]
	public void CrossJoinStringTableTest()
	{
		var query = from s in FromTable<sale>()
					from a in CrossJoinTable<article>("articles")
					select a;

		Monitor.Log(query);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Equal("articles as a", joins[0].TableInfo.ToSelectable().ToOneLineText());
		Assert.Equal("CrossJoinTable", joins[0].Relation);
		Assert.Null(joins[0].Condition);
	}


	public record struct sale(int sales_id, int article_id, int quantity);

	public record struct article(int article_id, string article_name, int price);
}
