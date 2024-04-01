using Carbunql.Analysis;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class InsertQueryTest
{
    private readonly QueryCommandMonitor Monitor;

    public InsertQueryTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void InsertValuesToInsertSelect()
    {
        var text = @"
INSERT INTO sale (sale_date,price,created_at) VALUES
     ('2023-01-01',160,'2024-01-11 14:29:01.618'),
     ('2023-03-12',200,'2024-01-11 14:29:01.618')";

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

        if (iq.TryConvertToInsertSelect(out var x))
        {
            iq = x;
        }

        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(34, lst.Count);
        Assert.Equal(expect, iq.ToText(), true, true, true);
    }

    [Fact]
    public void InsertSelectToInsertValues()
    {
        var text = @"INSERT INTO
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

        var expect = @"INSERT INTO
    SALE(sale_date, price, created_at)
VALUES
    ('2023-01-01', 160, '2024-01-11 14:29:01.618'),
    ('2023-03-12', 200, '2024-01-11 14:29:01.618')";

        var iq = InsertQueryParser.Parse(text);
        if (iq == null) throw new Exception();

        if (iq.TryConvertToInsertValues(out var x))
        {
            iq = x;
        }

        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(25, lst.Count);
        Assert.Equal(expect, iq.ToText(), true, true, true);
    }

    [Fact]
    public void InsertSelectToInsertValues_calc()
    {
        var text = @"INSERT INTO
    SALE(sale_date, price, created_at)
SELECT
    '2023-01-01' AS sale_date,
    160 + 10 AS price,
    '2024-01-11 14:29:01.618' AS created_at
UNION ALL
SELECT
    '2023-03-12' AS sale_date,
    200 * 20 AS price,
    '2024-01-11 14:29:01.618' AS created_at";

        var expect = @"INSERT INTO
    SALE(sale_date, price, created_at)
VALUES
    ('2023-01-01', 160 + 10, '2024-01-11 14:29:01.618'),
    ('2023-03-12', 200 * 20, '2024-01-11 14:29:01.618')";

        var iq = InsertQueryParser.Parse(text);
        if (iq == null) throw new Exception();

        if (iq.TryConvertToInsertValues(out var x))
        {
            iq = x;
        }

        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(29, lst.Count);
        Assert.Equal(expect, iq.ToText(), true, true, true);
    }

    [Fact]
    public void CatalogShemaTable()
    {
        var text = @"
INSERT INTO catalog.schema.sale (sale_date,price,created_at) VALUES
     ('2023-01-01',160,'2024-01-11 14:29:01.618'),
     ('2023-03-12',200,'2024-01-11 14:29:01.618')";

        var expect = @"INSERT INTO
    catalog.schema.sale (
        sale_date, price, created_at
    )
VALUES
    ('2023-01-01', 160, '2024-01-11 14:29:01.618'),
    ('2023-03-12', 200, '2024-01-11 14:29:01.618')";

        var iq = InsertQueryParser.Parse(text);
        if (iq == null) throw new Exception();

        if (iq.TryConvertToInsertSelect(out var x))
        {
            iq = x;
        }

        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(27, lst.Count);
        Assert.Equal(expect, iq.ToText(), true, true, true);
    }
}
