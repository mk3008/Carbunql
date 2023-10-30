using Carbunql;
using Xunit.Abstractions;
using static QueryBuilderByLinq.Sql;

namespace QueryBuilderByLinq.Test;

public class CommonTableTest
{
	private readonly QueryCommandMonitor Monitor;

	public CommonTableTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void DefaultTest()
	{
		var subq = from a in FromTable<table_a>() select new { ID = a.a_id, Text = a.text };

		var query = from cte in CommonTable(subq)
					from b in FromTable<table_a>(nameof(cte))
					where b.a_id == 1
					select b;

		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
WITH
    cte AS (
        SELECT
            a.a_id AS ID,
            a.text AS Text
        FROM
            table_a AS a
    )
SELECT 
	b.ID
	, b.Text
FROM
    cte AS b
WHERE
    b.a_id =  1";

		Assert.Equal(39, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void RelationTest()
	{
		var subq = from a in FromTable<table_a>() select new { ID = a.a_id, Text = a.text };

		var query = from cte in CommonTable(subq)
					from b in FromTable<table_a>(nameof(cte))
					from c in InnerJoinTable<table_a>(nameof(cte), x => b.a_id == x.a_id)
					where b.a_id == 1
					select new { b, c };

		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
WITH
    cte AS (
        SELECT
            a.a_id AS ID,
            a.text AS Text
        FROM
            table_a AS a
    )
SELECT
    b.ID,
    b.Text,
    c.ID,
    c.Text
FROM
    cte AS b
    INNER JOIN cte AS c ON b.a_id = c.a_id
WHERE
    b.a_id = 1";

		Assert.Equal(59, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void CTEsTest()
	{
		var sub_a1 = from a in FromTable<table_a>() select new { a.a_id, a.text };
		var sub_a2 = from a in FromTable<table_a>() select new { a.a_id, a.value };

		var query = from cte1 in CommonTable(sub_a1)
					from cte2 in CommonTable(sub_a2)
					from b in FromTable<table_a>(nameof(cte1))
					from c in InnerJoinTable<table_a>(nameof(cte2), x => b.a_id == x.a_id)
					where b.a_id == 1
					select new { b, c };

		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
WITH
    cte1 AS (
        SELECT
            a.a_id,
            a.text
        FROM
            table_a AS a
    ),
    cte2 AS (
        SELECT
            a.a_id,
            a.value
        FROM
            table_a AS a
    )
SELECT
    b.a_id,
    b.text,
    c.a_id,
    c.value
FROM
    cte1 AS b
    INNER JOIN cte2 AS c ON b.a_id = c.a_id
WHERE
    b.a_id = 1";

		Assert.Equal(72, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value);
}