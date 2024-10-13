using Carbunql.Fluent;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class FluentTest
{
    private readonly QueryCommandMonitor Monitor;

    public FluentTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void SelectTest()
    {
        var t = FluentTable.Create("test", "t");

        var sq = new SelectQuery()
            .From(t)
            .Select(t, "id");

        Monitor.Log(sq);

        var actual = sq.ToText();
        var expect = """
            SELECT
                t.id
            FROM
                test AS t
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void EqualTest()
    {
        var t = FluentTable.Create("select id, value from table", "t");

        var sq = new SelectQuery()
            .From(t)
            .SelectAll(t)
            .Equal("id", 1);

        Monitor.Log(sq);

        var actual = sq.ToText();
        var expect = """
            SELECT
                t.id,
                t.value
            FROM
                (
                    SELECT
                        id,
                        value
                    FROM
                        table
                    WHERE
                        id = 1
                ) AS t
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void EqualTest_NotFoundException()
    {
        var t = FluentTable.Create("select id, value from table", "t");

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            var sq = new SelectQuery()
                .From(t)
                .SelectAll(t)
                .Equal("table_id", 1);
        });

        var actual = ex.Message;
        var expect = "No matching QuerySource was found. column:table_id";

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void HasTest_True()
    {
        var t = FluentTable.Create("select id, value from table", "t");

        var sq = new SelectQuery()
            .From(t)
            .SelectAll(t)
            .If(x => x.HasColumn("id"), x => x.Equal("id", 1));

        Monitor.Log(sq);

        var actual = sq.ToText();
        var expect = """
            SELECT
                t.id,
                t.value
            FROM
                (
                    SELECT
                        id,
                        value
                    FROM
                        table
                    WHERE
                        id = 1
                ) AS t
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void HasTest_False()
    {
        var t = FluentTable.Create("select id, value from table", "t");

        var sq = new SelectQuery()
            .From(t)
            .SelectAll(t)
            .If(x => x.HasColumn("table_id"), x => x.Equal("table_id", 1));

        Monitor.Log(sq);

        var actual = sq.ToText();
        var expect = """
            SELECT
                t.id,
                t.value
            FROM
                (
                    SELECT
                        id,
                        value
                    FROM
                        table
                ) AS t
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void ExistsTest_Keys_Table()
    {
        var s = FluentTable.Create("select sale_id, product_id from sale", "s");
        var p = FluentTable.Create("select * from product", "p");

        var sq = new SelectQuery()
            .From(s)
            .SelectAll(s)
            .Exists(["product_id"], p);

        Monitor.Log(sq);

        var actual = sq.ToText();
        var expect = """
            SELECT
                s.sale_id,
                s.product_id
            FROM
                (
                    SELECT
                        sale_id,
                        product_id
                    FROM
                        sale
                    WHERE
                        EXISTS (
                            SELECT
                                *
                            FROM
                                (
                                    SELECT
                                        *
                                    FROM
                                        product
                                ) AS p
                            WHERE
                                p.product_id = sale.product_id
                        )
                ) AS s
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void ExistsTest_Table_Keys_Table()
    {
        var s = FluentTable.Create("select sale_id, product_id from sale", "s");
        var p = FluentTable.Create("select * from product", "p");

        var sq = new SelectQuery()
            .From(s)
            .SelectAll(s)
            .Exists(s, ["product_id"], p);

        Monitor.Log(sq);

        var actual = sq.ToText();
        var expect = """
            SELECT
                s.sale_id,
                s.product_id
            FROM
                (
                    SELECT
                        sale_id,
                        product_id
                    FROM
                        sale
                ) AS s
            WHERE
                EXISTS (
                    SELECT
                        *
                    FROM
                        (
                            SELECT
                                *
                            FROM
                                product
                        ) AS p
                    WHERE
                        p.product_id = s.product_id
                )
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void ExistsTest_Keys_TableName()
    {
        var s = FluentTable.Create("select sale_id, product_id from sale", "s");

        var sq = new SelectQuery()
            .From(s)
            .SelectAll(s)
            .Exists(["product_id"], "product");

        Monitor.Log(sq);

        var actual = sq.ToText();
        var expect = """
            SELECT
                s.sale_id,
                s.product_id
            FROM
                (
                    SELECT
                        sale_id,
                        product_id
                    FROM
                        sale
                    WHERE
                        EXISTS (
                            SELECT
                                *
                            FROM
                                product AS x
                            WHERE
                                x.product_id = sale.product_id
                        )
                ) AS s
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void ExistsTest_Table_Keys_TableName()
    {
        var s = FluentTable.Create("select sale_id, product_id from sale", "s");

        var sq = new SelectQuery()
            .From(s)
            .SelectAll(s)
            .Exists(s.Alias, ["product_id"], "product");

        Monitor.Log(sq);

        var actual = sq.ToText();
        var expect = """
            SELECT
                s.sale_id,
                s.product_id
            FROM
                (
                    SELECT
                        sale_id,
                        product_id
                    FROM
                        sale
                ) AS s
            WHERE
                EXISTS (
                    SELECT
                        *
                    FROM
                        product AS x
                    WHERE
                        x.product_id = s.product_id
                )
            """;

        Assert.Equal(expect, actual);
    }
}
