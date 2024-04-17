using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class InsertQueryParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public InsertQueryParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void InsertValues()
    {
        var text = @"
INSERT INTO sale (sale_date,price,created_at) VALUES
     ('2023-01-01',160,'2024-01-11 14:29:01.618'),
     ('2023-03-12',200,'2024-01-11 14:29:01.618')";

        var expect = @"INSERT INTO
    sale(sale_date, price, created_at)
VALUES
    ('2023-01-01', 160, '2024-01-11 14:29:01.618'),
    ('2023-03-12', 200, '2024-01-11 14:29:01.618')";

        var iq = InsertQueryParser.Parse(text);
        if (iq == null) throw new Exception();

        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(25, lst.Count);
        Assert.Equal(expect, iq.ToText(), true, true, true);
    }

    [Fact]
    public void InsertSelect()
    {
        var text = @"
INSERT INTO sale (sale_date,price,created_at)
select
'2023-01-01' as sale_date,
160 as price,
'2024-01-11 14:29:01.618' as created_at
union all
select
'2023-03-12' as sale_date,
200 as price,
'2024-01-11 14:29:01.618' as created_at";

        var expect = @"INSERT INTO
    SALE(sale_date, price, created_at)
SELECT
    '2023-01-01' AS sale_date,
    160 AS price,
    '2024-01-11 14:29:01.618' AS created_at
UNION ALL
SELECT
    '2023-03-12' AS sale_date,
    200 AS price,
    '2024-01-11 14:29:01.618' AS created_at";

        var iq = InsertQueryParser.Parse(text);
        if (iq == null) throw new Exception();

        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(34, lst.Count);
        Assert.Equal(expect, iq.ToText(), true, true, true);
    }

    [Fact]
    public void InsertValuesReturning()
    {
        var text = @"
INSERT INTO sale (sale_date,price,created_at) VALUES
     ('2023-01-01',160,'2024-01-11 14:29:01.618'),
     ('2023-03-12',200,'2024-01-11 14:29:01.618')
returning sale_id, sale_date, price, created_at";

        var expect = @"INSERT INTO
    sale(sale_date, price, created_at)
VALUES
    ('2023-01-01', 160, '2024-01-11 14:29:01.618'),
    ('2023-03-12', 200, '2024-01-11 14:29:01.618')
RETURNING
    sale_id, sale_date, price, created_at";

        var iq = InsertQueryParser.Parse(text);
        if (iq == null) throw new Exception();

        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(33, lst.Count);
        Assert.Equal(expect, iq.ToText(), true, true, true);
    }

    [Fact]
    public void InsertWithSelect()
    {
        var text = @"
INSERT INTO sale (sale_date,price,created_at)
with
dat as (
	select
	'2023-01-01' as sale_date,
	160 as price,
	'2024-01-11 14:29:01.618' as created_at
	union all
	select
	'2023-03-12' as sale_date,
	200 as price,
	'2024-01-11 14:29:01.618' as created_at
)
select
sale_date,price,created_at
from
dat
"
;

        var expect = @"INSERT INTO
    sale(sale_date, price, created_at)
WITH
    dat AS (
        SELECT
            '2023-01-01' AS sale_date,
            160 AS price,
            '2024-01-11 14:29:01.618' AS created_at
        UNION ALL
        SELECT
            '2023-03-12' AS sale_date,
            200 AS price,
            '2024-01-11 14:29:01.618' AS created_at
    )
SELECT
    sale_date,
    price,
    created_at
FROM
    dat";

        var iq = InsertQueryParser.Parse(text);
        if (iq == null) throw new Exception();

        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(47, lst.Count);
        Assert.Equal(expect, iq.ToText(), true, true, true);
    }

    [Fact]
    public void WithInsertSelect()
    {
        var text = @"
with
dat as (
	select
	'2023-01-01' as sale_date,
	160 as price,
	'2024-01-11 14:29:01.618' as created_at
	union all
	select
	'2023-03-12' as sale_date,
	200 as price,
	'2024-01-11 14:29:01.618' as created_at
)
INSERT INTO sale (sale_date,price,created_at)
select
sale_date,price,created_at
from
dat
"
;

        // NOTE
        // Although this is a preliminary specification,
        // insert queries themselves do not allow CTEs.
        // So if her CTE is mentioned in the insert query,
        // it will be forced to be treated as her CTE in the select query.
        var expect = @"INSERT INTO
    sale(sale_date, price, created_at)
WITH
    dat AS (
        SELECT
            '2023-01-01' AS sale_date,
            160 AS price,
            '2024-01-11 14:29:01.618' AS created_at
        UNION ALL
        SELECT
            '2023-03-12' AS sale_date,
            200 AS price,
            '2024-01-11 14:29:01.618' AS created_at
    )
SELECT
    sale_date,
    price,
    created_at
FROM
    dat";

        var iq = InsertQueryParser.Parse(text);
        if (iq == null) throw new Exception();

        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(47, lst.Count);
        Assert.Equal(expect, iq.ToText(), true, true, true);
    }
}