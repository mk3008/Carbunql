using Carbunql;
using QueryBuilderByLinq.Analysis;
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

	private void WriteLog(IQueryable query)
	{
		var from = FromTableInfoParser.Parse(query.Expression);
		if (from != null)
		{
			Output.WriteLine($"from : {from.Alias}");
			Output.WriteLine("--------------------");
		}
		else
		{
			Output.WriteLine($"from : [NULL]");
		}

		var text = ExpressionReader.Analyze(query.Expression);
		Output.WriteLine(text);
		Output.WriteLine("--------------------");



	}

	[Fact]
	public void FromSelect()
	{
		var subq = from a in FromTable<table_a>() select new { ID = a.a_id, Text = a.text };

		var query = from cte in CommonTable(subq)
					from b in FromTable(cte)
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
    b.ID,
    b.Text
FROM
    cte AS b";

		Assert.Equal(33, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void FromWhereSelect()
	{
		var subq = from a in FromTable<table_a>() select new { ID = a.a_id, Text = a.text };

		var query = from cte in CommonTable(subq)
					from b in FromTable(cte)
					where b.ID == 1
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
    b.ID,
    b.Text
FROM
    cte AS b
WHERE
    b.ID = 1";

		Assert.Equal(39, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void RelationTest()
	{
		var subq = from a in FromTable<table_a>() select new { ID = a.a_id, Text = a.text };

		var query = from cte in CommonTable(subq)
					from b in FromTable(cte)
					from c in InnerJoinTable(cte, x => b.ID == x.ID)
					where b.ID == 1
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
    INNER JOIN cte AS c ON b.ID = c.ID
WHERE
    b.ID = 1";

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
					from b in FromTable(cte1)
					from c in InnerJoinTable(cte2, x => b.a_id == x.a_id)
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

	[Fact]
	public void CTEsTest3()
	{
		var sub_a1 = from a in FromTable<table_a>() select new { a.a_id, a.text };
		var sub_a2 = from a in FromTable<table_a>() select new { a.a_id, a.value };
		var sub_a3 = from a in FromTable<table_a>() select a;

		var query = from cte1 in CommonTable(sub_a1)
					from cte2 in CommonTable(sub_a2)
					from cte3 in CommonTable(sub_a3)
					from b in FromTable(cte1)
					from c in InnerJoinTable(cte2, x => b.a_id == x.a_id)
					from d in LeftJoinTable(cte3, x => b.a_id == x.a_id)
					from e in CrossJoinTable(cte3)
					where b.a_id == 1
					select new { b, c, d, e };

		var sq = query.ToQueryAsPostgres();

		Monitor.Log(query);
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
    ),
    cte3 AS (
        SELECT
            a.a_id,
            a.text,
            a.value
        FROM
            table_a AS a
    )
SELECT
    b.a_id,
    b.text,
    c.a_id,
    c.value,
    d.a_id,
    d.text,
    d.value,
    e.a_id,
    e.text,
    e.value,
FROM
    cte1 AS b
    INNER JOIN cte2 AS c ON b.a_id = c.a_id
    LEFT JOIN cte3 AS d ON b.a_id = d.a_id
    CROSS JOIN cte3 AS e
WHERE
    b.a_id = 1";

		Assert.Equal(117, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value);
}