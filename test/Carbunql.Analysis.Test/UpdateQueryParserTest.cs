using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class UpdateQueryParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public UpdateQueryParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void QueryCommandableParserTest()
    {
        var text = @"UPDATE table_a SET value = 1";
        var q = QueryCommandableParser.Parse(text);

        var actual = q.ToText();
        var expect = """
            UPDATE
                table_a
            SET
                value = 1
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void QueryCommandableParserTest_Cte()
    {
        var text = @"
        WITH cte AS (
            SELECT id, value FROM table_b WHERE value > 10
        )
        UPDATE table_a
        SET value = cte.value
        FROM cte
        WHERE table_a.id = cte.id";
        var q = QueryCommandableParser.Parse(text);

        var actual = q.ToText();
        var expect = """
            WITH
                cte AS (
                    SELECT
                        id,
                        value
                    FROM
                        table_b
                    WHERE
                        value > 10
                )
            UPDATE
                table_a
            SET
                value = cte.value
            FROM
                cte
            WHERE
                table_a.id = cte.id
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void UpdateAllTest()
    {
        var text = @"UPDATE table_a SET value = 1";
        var q = UpdateQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
            UPDATE
                table_a
            SET
                value = 1
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void UpdateWithWhereTest()
    {
        var text = @"UPDATE table_a SET value = 1 WHERE id = 10";
        var q = UpdateQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
            UPDATE
                table_a
            SET
                value = 1
            WHERE
                id = 10
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void UpdateMultipleColumnsTest()
    {
        var text = @"UPDATE table_a SET value = 1, name = 'John'";
        var q = UpdateQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
            UPDATE
                table_a
            SET
                value = 1,
                name = 'John'
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void UpdateWithSubqueryTest()
    {
        var text = @"UPDATE table_a SET value = (SELECT max(value) FROM table_b)";
        var q = UpdateQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
            UPDATE
                table_a
            SET
                value = (
                    SELECT
                        MAX(value)
                    FROM
                        table_b
                )
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void UpdateWithJoinTest()
    {
        var text = @"UPDATE table_a SET value = 1 FROM table_b WHERE table_a.id = table_b.id";
        var q = UpdateQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
            UPDATE
                table_a
            SET
                value = 1
            FROM
                table_b
            WHERE
                table_a.id = table_b.id
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void UpdateWithReturningTest()
    {
        var text = @"UPDATE table_a SET value = 1 WHERE id = 10 RETURNING id, value";
        var q = UpdateQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
            UPDATE
                table_a
            SET
                value = 1
            WHERE
                id = 10
            RETURNING
                id, value
            """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void UpdateWithTableAliasTest()
    {
        var text = @"UPDATE table_a AS a SET value = 1 WHERE a.id = 10";
        var q = UpdateQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
        UPDATE
            table_a AS a
        SET
            value = 1
        WHERE
            a.id = 10
        """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void UpdateWithCteTest()
    {
        var text = @"
        WITH cte AS (
            SELECT id, value FROM table_b WHERE value > 10
        )
        UPDATE table_a
        SET value = cte.value
        FROM cte
        WHERE table_a.id = cte.id";

        var q = UpdateQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
        WITH
            cte AS (
                SELECT
                    id,
                    value
                FROM
                    table_b
                WHERE
                    value > 10
            )
        UPDATE
            table_a
        SET
            value = cte.value
        FROM
            cte
        WHERE
            table_a.id = cte.id
        """;

        Assert.Equal(expect, actual);
    }

    [Fact]
    public void UpdateWithInnerJoinTest()
    {
        var text = @"
        UPDATE table_a a
        SET value = table_b.value
        FROM table_b b
        INNER JOIN table_c c ON b.id = c.b_id
        WHERE a.id = b.id";

        var q = UpdateQueryParser.Parse(text);

        Monitor.Log(q);

        var actual = q.ToText();
        var expect = """
        UPDATE
            table_a AS a
        SET
            value = table_b.value
        FROM
            table_b AS b
            INNER JOIN table_c AS c ON b.id = c.b_id
        WHERE
            a.id = b.id
        """;

        Assert.Equal(expect, actual);
    }
}
