using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class ToTextTest
{
	private ITestOutputHelper Output;

	public ToTextTest(ITestOutputHelper output)
	{
		Output = output;
	}

	private string TruncateControlString(string text)
	{
		return text.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToLower();
	}

	[Fact]
	public void ToText()
	{
		var sql = @"
/*
  :id = 1
  :value = 'test'
*/
select a.column_1 as c1 from table_a as a";
		var sq = new SelectQuery(sql);
		sq.AddParameter(":id", 1);
		sq.AddParameter(":value", "test");

		Output.WriteLine(sq.ToText());

		Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
	}

	[Fact]
	public void ToOneLineText()
	{
		var sql = @"
/*
  :id = 1
  :value = 'test'
*/
select a.column_1 as c1 from table_a as a";
		var sq = new SelectQuery(sql);
		sq.AddParameter(":id", 1);
		sq.AddParameter(":value", "test");

		Output.WriteLine(sq.ToOneLineText());

		Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToOneLineText()));
	}

	[Fact]
	public void ToOneLineText_identity()
	{
		var sql = @"WITH
    dat (
        line_id, name, unit_price, quantity, tax_rate
    ) AS (
        VALUES
            (1, 'apple', 105, 5, 0.07),
            (2, 'orange', 203, 3, 0.07),
            (3, 'banana', 233, 9, 0.07),
            (4, 'tea', 309, 7, 0.08),
            (5, 'coffee', 555, 9, 0.08),
            (6, 'cola', 456, 2, 0.08)
    ),
    detail AS (
        SELECT
            q.*,
            TRUNC(q.price * (1 + q.tax_rate)) - q.price AS tax,
            q.price * (1 + q.tax_rate) - q.price AS raw_tax
        FROM
            (
                SELECT
                    dat.*,
                    (dat.unit_price * dat.quantity) AS price
                FROM
                    dat
            ) AS q
    ),
    tax_summary AS (
        SELECT
            d.tax_rate,
            TRUNC(SUM(raw_tax)) AS total_tax
        FROM
            detail AS d
        GROUP BY
            d.tax_rate
    )
SELECT
    line_id,
    name,
    unit_price,
    quantity,
    tax_rate,
    price,
    price + tax AS tax_included_price,
    tax
FROM
    (
        SELECT
            line_id,
            name,
            unit_price,
            quantity,
            tax_rate,
            price,
            tax + adjust_tax AS tax
        FROM
            (
                SELECT
                    q.*,
                    CASE
                        WHEN q.total_tax - q.cumulative >= q.priority THEN 1
                        ELSE 0
                    END AS adjust_tax
                FROM
                    (
                        SELECT
                            d.*,
                            s.total_tax,
                            SUM(d.tax) OVER(
                                PARTITION BY
                                    d.tax_rate
                            ) AS cumulative,
                            ROW_NUMBER() OVER(
                                PARTITION BY
                                    d.tax_rate
                                ORDER BY
                                    d.raw_tax % 1 DESC,
                                    d.line_id
                            ) AS priority
                        FROM
                            detail AS d
                            INNER JOIN tax_summary AS s ON d.tax_rate = s.tax_rate
                    ) AS q
            ) AS q
    ) AS q
ORDER BY
    line_id";
		var sq = new SelectQuery(sql);
		Output.WriteLine(sq.ToText());
		Output.WriteLine(sq.ToOneLineText());

		var sq2 = new SelectQuery(sq.ToOneLineCommand().CommandText);
		Assert.Equal(sql, sq2.ToCommand().CommandText);
	}
}
