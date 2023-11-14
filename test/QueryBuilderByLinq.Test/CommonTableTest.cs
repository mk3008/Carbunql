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
		var from = SelectableTableParser.Parse(query.Expression);
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

		var sq = query.ToSelectQuery();

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

		var sq = query.ToSelectQuery();

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
					select b;

		var sq = query.ToSelectQuery();

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
    INNER JOIN cte AS c ON b.ID = x.ID
WHERE
    b.ID = 1";

		Assert.Equal(51, sq.GetTokens().ToList().Count);
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
					select b;

		var sq = query.ToSelectQuery();

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
    b.text
FROM
    cte1 AS b
    INNER JOIN cte2 AS c ON b.a_id = x.a_id
WHERE
    b.a_id = 1";

		Assert.Equal(64, sq.GetTokens().ToList().Count);
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
					from bb in FromTable(cte1)
					from cc in InnerJoinTable(cte2, x => bb.a_id == x.a_id)
					from dd in LeftJoinTable(cte3, x => bb.a_id == x.a_id)
					from ee in CrossJoinTable(cte3)
					where cc.a_id == 1
					select cc;

		Monitor.Log(query);

		var sq = query.ToSelectQuery();

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
    cc.a_id,
    cc.value
FROM
    cte1 AS bb
    INNER JOIN cte2 AS cc ON bb.a_id = x.a_id
    LEFT JOIN cte3 AS dd ON bb.a_id = x.a_id
    CROSS JOIN cte3 AS ee
WHERE
    cc.a_id = 1";

		Assert.Equal(101, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value);
}