using Xunit.Abstractions;

namespace Carbunql.Fluent.Test;

public class FluentWhereTest
{
    private readonly QueryCommandMonitor Monitor;

    public FluentWhereTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void EqualTest()
    {
        var sq = new SelectQuery("""
            select
                a.id
                , a.value
            from
                table_a a
            """)
            .Equal("id", 1);
        ;

        Monitor.Log(sq);

        var expect = """
            SELECT
                a.id,
                a.value
            FROM
                table_a AS a
            WHERE
                a.id = 1
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void EqualTest_ExtractYear()
    {
        var sq = new SelectQuery("""
            SELECT 
                o.order_id,
                c.customer_name,
                o.order_date,
                EXTRACT(YEAR FROM o.order_date) AS order_year,
                EXTRACT(MONTH FROM o.order_date) AS order_month,
                o.amount
            FROM 
                orders o
            JOIN 
                customers c ON o.customer_id = c.customer_id
            """)
            .Equal("order_year", 1);
        ;

        Monitor.Log(sq);

        var expect = """
            SELECT
                o.order_id,
                c.customer_name,
                o.order_date,
                EXTRACT(YEAR FROM o.order_date) AS order_year,
                EXTRACT(MONTH FROM o.order_date) AS order_month,
                o.amount
            FROM
                orders AS o
                JOIN customers AS c ON o.customer_id = c.customer_id
            WHERE
                EXTRACT(YEAR FROM o.order_date) = 1
            """;

        Assert.Equal(expect, sq.ToText());
    }
}