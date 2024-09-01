using Xunit.Abstractions;

namespace Carbunql.Fluent.Test;

public class ConvertTest
{
    private readonly QueryCommandMonitor Monitor;

    public ConvertTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void ToSubQueryTest()
    {
        var sql = """
            select
                a.id
            from
                table_a as a
            """;

        var sq = SelectQuery.Parse(sql)
            .ToSubQuery("s", out var s)
            .SelectAll(s);

        Monitor.Log(sq);

        var expect = """
            SELECT
                s.id
            FROM
                (
                    SELECT
                        a.id
                    FROM
                        table_a AS a
                ) AS s
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void ToCteTest()
    {
        var sql = """
            select
                a.id
            from
                table_a as a
            """;

        var sq = SelectQuery.Parse(sql)
            .ToCteQuery("cte", "a", out var a)
            .SelectAll(a);

        Monitor.Log(sq);

        var expect = """
            WITH
                cte AS (
                    SELECT
                        a.id
                    FROM
                        table_a AS a
                )
            SELECT
                a.id
            FROM
                cte AS a
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void ToCountQuery()
    {
        var sql = """
            select
                a.id
            from
                table_a as a
            """;

        var sq = SelectQuery.Parse(sql)
            .ToCountQuery();

        Monitor.Log(sq);

        var expect = """
            SELECT
                COUNT(*) AS row_count
            FROM
                (
                    SELECT
                        a.id
                    FROM
                        table_a AS a
                ) AS q
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void ToCreateTableQuery()
    {
        var sql = """
            select
                a.id
            from
                table_a as a
            """;

        var cq = SelectQuery.Parse(sql)
            .ToCreateTableQuery("tmp", true);

        Monitor.Log(cq);

        var expect = """
            CREATE TEMPORARY TABLE tmp
            AS
            SELECT
                a.id
            FROM
                table_a AS a
            """;

        Assert.Equal(expect, cq.ToText());
    }

    [Fact]
    public void ToInertQuery()
    {
        var sql = """
            select
                a.id
            from
                table_a as a
            """;

        var iq = SelectQuery.Parse(sql)
            .ToInsertQuery("new_table");

        Monitor.Log(iq);

        var expect = """
            INSERT INTO
                new_table(id)
            SELECT
                a.id
            FROM
                table_a AS a
            """;

        Assert.Equal(expect, iq.ToText());
    }

    [Fact]
    public void ToUpdateQuery()
    {
        var sql = """
            select
                a.id
                , a.value
            from
                table_a as a
            """;

        var up = SelectQuery.Parse(sql)
            .ToUpdateQuery("new_table", ["id"]);

        Monitor.Log(up);

        var expect = """
            UPDATE
                new_table AS d
            SET
                value = q.value
            FROM
                (
                    SELECT
                        a.id,
                        a.value
                    FROM
                        table_a AS a
                ) AS q
            WHERE
                d.id = q.id
            """;

        Assert.Equal(expect, up.ToText());
    }

    [Fact]
    public void ToDeleteQuery()
    {
        var sql = """
            select
                a.id
                , a.value
            from
                table_a as a
            """;

        var dq = SelectQuery.Parse(sql)
            .ToDeleteQuery("new_table", ["id"]);

        Monitor.Log(dq);

        var expect = """
            DELETE FROM
                new_table AS d
            WHERE
                (d.id) IN (
                    SELECT
                        q.id
                    FROM
                        (
                            SELECT
                                a.id,
                                a.value
                            FROM
                                table_a AS a
                        ) AS q
                )
            """;

        Assert.Equal(expect, dq.ToText());
    }

    [Fact]
    public void ToMergeQuery()
    {
        var sql = """
            select
                a.id
                , a.value
            from
                table_a as a
            """;

        var mq = SelectQuery.Parse(sql)
            .ToMergeQuery("new_table", ["id"])
            .MatchedUpdate()
            .NotMatchedInsert();

        Monitor.Log(mq);

        var expect = """
            MERGE INTO
                new_table AS d
            USING
                (
                    SELECT
                        a.id,
                        a.value
                    FROM
                        table_a AS a
                ) AS s ON d.id = s.id
            WHEN MATCHED THEN
                UPDATE SET
                    value = s.value
            WHEN NOT MATCHED THEN
                INSERT (
                    id, value
                ) VALUES (
                    s.id, s.value
                )
            """;

        Assert.Equal(expect, mq.ToText());
    }
}