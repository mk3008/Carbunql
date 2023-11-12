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

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Empty(joins);

		var sql = @"
SELECT
    a.sales_id,
    a.article_id,
    a.quantity
FROM
    sale AS a
";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}

	[Fact]
	public void InnerJoinTypeTableTest()
	{
		var query = from s in FromTable<sale>()
					from a in InnerJoinTable<article>(x => s.article_id == x.article_id)
					from c in InnerJoinTable<category>(x => a.category_id == x.category_id)
					select a;

		Monitor.Log(query);

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Equal(2, joins.Count);

		Assert.Equal("article as a", joins[0].TableInfo.ToSelectable().ToOneLineText());
		Assert.Equal("inner join", joins[0].Relation);
		Assert.Equal("s.article_id = a.article_id", joins[0].Condition!.ToOneLineText());

		var sql = @"
SELECT
    a.article_id,
    a.category_id,
    a.article_name,
    a.price
FROM
    sale AS s
    INNER JOIN article AS a ON s.article_id = a.article_id
    INNER JOIN category AS c ON a.category_id = c.category_id
";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}

	[Fact]
	public void InnerJoinStringTableTest()
	{
		var query = from s in FromTable<sale>("sales")
					from a in InnerJoinTable<article>("articles", x => s.article_id == x.article_id)
					from c in InnerJoinTable<category>("categories", x => a.category_id == x.category_id)
					select a;

		Monitor.Log(query);

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Equal(2, joins.Count);

		Assert.Equal("articles as a", joins[0].TableInfo.ToSelectable().ToOneLineText());
		Assert.Equal("inner join", joins[0].Relation);
		Assert.Equal("s.article_id = a.article_id", joins[0].Condition!.ToOneLineText());

		var sql = @"
SELECT
    a.article_id,
    a.category_id,
    a.article_name,
    a.price
FROM
    sales AS s
    INNER JOIN articles AS a ON s.article_id = a.article_id
    INNER JOIN categories AS c ON a.category_id = c.category_id
";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}

	[Fact]
	public void LeftJoinTest()
	{
		var query = from s in FromTable<sale>()
					from a in LeftJoinTable<article>(x => s.article_id == x.article_id)
					from c in LeftJoinTable<category>(x => a.category_id == x.category_id)
					select a;

		Monitor.Log(query);

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Equal("article as a", joins[0].TableInfo.ToSelectable().ToOneLineText());
		Assert.Equal("left join", joins[0].Relation);
		Assert.Equal("s.article_id = a.article_id", joins[0].Condition!.ToOneLineText());

		var sql = @"
SELECT
    a.article_id,
    a.category_id,
    a.article_name,
    a.price
FROM
    sale AS s
    LEFT JOIN article AS a ON s.article_id = a.article_id
    LEFT JOIN category AS c ON a.category_id = c.category_id
";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}

	[Fact]
	public void LeftJoinStringTableTest()
	{
		var query = from s in FromTable<sale>()
					from a in LeftJoinTable<article>("articles", x => s.article_id == x.article_id)
					from c in LeftJoinTable<category>("categories", x => a.category_id == x.category_id)
					select a;

		Monitor.Log(query);

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Equal(2, joins.Count);

		Assert.Equal("articles as a", joins[0].TableInfo.ToSelectable().ToOneLineText());
		Assert.Equal("left join", joins[0].Relation);
		Assert.Equal("s.article_id = a.article_id", joins[0].Condition!.ToOneLineText());

		var sql = @"
SELECT
    a.article_id,
    a.category_id,
    a.article_name,
    a.price
FROM
    sale AS s
    LEFT JOIN articles AS a ON s.article_id = a.article_id
    LEFT JOIN categories AS c ON a.category_id = c.category_id
";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}

	[Fact]
	public void CrossJoinTest()
	{
		var query = from s in FromTable<sale>()
					from a in CrossJoinTable<article>()
					from c in CrossJoinTable<category>()
					select a;

		Monitor.Log(query);

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Equal("article as a", joins[0].TableInfo.ToSelectable().ToOneLineText());
		Assert.Equal("cross join", joins[0].Relation);
		Assert.Null(joins[0].Condition);

		var sql = @"
SELECT
    a.article_id,
    a.category_id,
    a.article_name,
    a.price
FROM
    sale AS s
    CROSS JOIN article AS a
    CROSS JOIN category AS c
";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}

	[Fact]
	public void CrossJoinStringTableTest()
	{
		var query = from s in FromTable<sale>("sales")
					from a in CrossJoinTable<article>("articles")
					from c in CrossJoinTable<category>("categories")
					select a;

		Monitor.Log(query);

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var joins = JoinTableInfoParser.Parse(query.Expression);

		Assert.Equal("articles as a", joins[0].TableInfo.ToSelectable().ToOneLineText());
		Assert.Equal("cross join", joins[0].Relation);
		Assert.Null(joins[0].Condition);

		var sql = @"
SELECT
    a.article_id,
    a.category_id,
    a.article_name,
    a.price
FROM
    sales AS s
    CROSS JOIN articles AS a
    CROSS JOIN categories AS c
";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}

	public record struct sale(int sales_id, int article_id, int quantity);

	public record struct article(int article_id, int category_id, string article_name, int price);

	public record struct category(int category_id, string category_name);
}
