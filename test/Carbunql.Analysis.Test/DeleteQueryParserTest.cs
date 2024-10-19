using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class DeleteQueryParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public DeleteQueryParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void QueryCommandableParserTest()
    {
        var text = @"DELETE FROM table_a WHERE id = 1";
        var q = QueryCommandableParser.Parse(text);

        var actual = q.ToText();
        var expect = """
            DELETE FROM
                table_a
            WHERE
                id = 1
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void QueryCommandableParserTest_Cte()
    {
        var text = @"
        WITH cte AS (
            SELECT id FROM table_b WHERE status = 'inactive'
        )
        DELETE FROM table_a
        WHERE table_a.id IN (SELECT id FROM cte)";
        var q = QueryCommandableParser.Parse(text);

        var actual = q.ToText();
        var expect = """
            WITH
                cte AS (
                    SELECT
                        id
                    FROM
                        table_b
                    WHERE
                        status = 'inactive'
                )
            DELETE FROM
                table_a
            WHERE
                table_a.id IN (
                    SELECT
                        id
                    FROM
                        cte
                )
            """;

        Assert.Equal(expect, actual);
    }
    [Fact]
    public void DeleteBasicTest()
    {
        var text = @"DELETE FROM table_a WHERE id = 1";
        var q = DeleteQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
        DELETE FROM
            table_a
        WHERE
            id = 1
        """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void DeleteWithAliasTest()
    {
        var text = @"DELETE FROM table_a AS a WHERE a.id = 1";
        var q = DeleteQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
        DELETE FROM
            table_a AS a
        WHERE
            a.id = 1
        """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void DeleteWithUsingTest()
    {
        var text = @"
        DELETE FROM table_a as a
        USING table_b as b
        WHERE a.id = b.id
        AND b.status = 'inactive'";

        var q = DeleteQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
        DELETE FROM
            table_a AS a
        USING
            table_b AS b
        WHERE
            a.id = b.id
            AND b.status = 'inactive'
        """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void DeleteWithMultipleTablesTest()
    {
        var text = @"
            DELETE FROM orders AS o
            USING customers AS c, payments AS p
            WHERE o.customer_id = c.id
              AND c.payment_id = p.id
              AND p.status = 'completed';
        ";
        var q = DeleteQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
            DELETE FROM
                orders AS o
            USING
                customers AS c, payments AS p
            WHERE
                o.customer_id = c.id
                AND c.payment_id = p.id
                AND p.status = 'completed'
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void DeleteWithReturningColumnsTest()
    {
        var text = @"DELETE FROM table_a WHERE id = 1 RETURNING id, value";
        var q = DeleteQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
        DELETE FROM
            table_a
        WHERE
            id = 1
        RETURNING
            id, value
        """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void DeleteWithCteTest()
    {
        var text = @"
        WITH cte AS (
            SELECT id FROM table_b WHERE status = 'inactive'
        )
        DELETE FROM table_a
        WHERE table_a.id IN (SELECT id FROM cte)";

        var q = DeleteQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
        WITH
            cte AS (
                SELECT
                    id
                FROM
                    table_b
                WHERE
                    status = 'inactive'
            )
        DELETE FROM
            table_a
        WHERE
            table_a.id IN (
                SELECT
                    id
                FROM
                    cte
            )
        """;

        Assert.Equal(expect, actual);
    }

}
