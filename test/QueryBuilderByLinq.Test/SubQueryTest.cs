using Carbunql;
using Xunit.Abstractions;
using static QueryBuilderByLinq.Sql;

namespace QueryBuilderByLinq.Test;

public class SubQueryTest
{
	private readonly QueryCommandMonitor Monitor;

	public SubQueryTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void DefaultTest()
	{
		var subq = from a in From<table_a>() select new { ID = a.a_id, Text = a.text };
		var query = from x in From(subq) select new { x.ID, x.Text };

		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    x.ID,
    x.Text
FROM
    (
        SELECT
            a.a_id AS ID,
            a.text AS Text
        FROM
            table_a AS a
    ) AS x";

		Assert.Equal(29, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void JoinTest()
	{
		var suba = from a in From<table_a>() select new { a.a_id, a_text = a.text };
		var subx = from a in From<table_a>() select new { x_id = a.a_id, x_text = a.text };
		var suby = from a in From<table_a>() select new { y_id = a.a_id, y_text = a.text };
		var subz = from a in From<table_a>() select new { z_id = a.a_id, z_text = a.text };

		var query = from a in From(suba)
					from x in InnerJoin(subx, x => a.a_id == x.x_id)
					from y in LeftJoin(suby, y => a.a_id == y.y_id)
					from z in CrossJoin(subz)
					select new { a, x, y, z };

		var sq = query.ToQueryAsPostgres();

		Monitor.Log(sq);

		var sql = @"
SELECT
    z.z_id,
    z.z_text,
    a.a_id,
    a.a_text,
    x.x_id,
    x.x_text,
    y.y_id,
    y.y_text
FROM
    (
        SELECT
            a.a_id,
            a.text AS a_text
        FROM
            table_a AS a
    ) AS a
    INNER JOIN (
        SELECT
            a.a_id AS x_id,
            a.text AS x_text
        FROM
            table_a AS a
    ) AS x ON a.a_id = x.x_id
    LEFT JOIN (
        SELECT
            a.a_id AS y_id,
            a.text AS y_text
        FROM
            table_a AS a
    ) AS y ON a.a_id = y.y_id
    CROSS JOIN (
        SELECT
            a.a_id AS z_id,
            a.text AS z_text
        FROM
            table_a AS a
    ) AS z";

		Assert.Equal(130, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value);
}