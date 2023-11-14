using Carbunql.Postgres.Analysis;
using Xunit.Abstractions;
using static Carbunql.Postgres.Analysis.Sql;

namespace Carbunql.Postgres.Test.Analysis;

public class WhereParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public WhereParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void Nothing()
	{
		var query = from a in FromTable<table_a>()
					select a;

		Monitor.Log(query);

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var where = WhereValueParser.Parse(query.Expression);
		Assert.Null(where);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a
";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}

	[Fact]
	public void Simple()
	{
		var query = from a in FromTable<table_a>()
					where a.a_id == 1
					select a;

		Monitor.Log(query);

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var where = WhereValueParser.Parse(query.Expression);
		Assert.Equal("a.a_id = 1", where!.ToOneLineText());

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value
FROM
    table_a AS a
WHERE
    a.a_id = 1
";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}

	[Fact]
	public void Cte()
	{
		var subquery = from a in FromTable<table_a>() select new { a.a_id };

		var query = from cte in CommonTable(subquery)
					from a in FromTable(cte)
					where a.a_id == 1
					select a;

		Monitor.Log(query);

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var where = WhereValueParser.Parse(query.Expression);
		Assert.Equal("a.a_id = 1", where!.ToOneLineText());

		var sql = @"
WITH
    cte AS (
        SELECT
            a.a_id
        FROM
            table_a AS a
    )
SELECT
    a.a_id
FROM
    cte AS a
WHERE
    a.a_id = 1
";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}

	[Fact]
	public void Join()
	{
		var query = from a in FromTable<table_a>()
					from b in CrossJoinTable<table_a>()
					where a.a_id == 1 && b.text == "test"
					select new { a.a_id, b.text };

		Monitor.Log(query);

		var sq = query.ToSelectQuery();
		Monitor.Log(sq);

		var where = WhereValueParser.Parse(query.Expression);
		Assert.Equal("(a.a_id = 1 and b.text = 'test')", where!.ToOneLineText());

		var sql = @"
SELECT
    a.a_id,
    b.text
FROM
    table_a AS a
    CROSS JOIN table_a AS b
WHERE
    (a.a_id = 1 AND b.text = 'test')
";
		Assert.Equal(sql.RemoveControlChar(), sq.ToText().RemoveControlChar());
	}


	public record struct table_a(int a_id, string text, int value);

	public record struct sale(int sales_id, int article_id, int quantity);

	public record struct article(int article_id, string article_name, int price);
}