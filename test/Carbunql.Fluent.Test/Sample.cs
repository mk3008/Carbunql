using Xunit.Abstractions;

namespace Carbunql.Fluent.Test;

public class Sample
{
    private readonly QueryCommandMonitor Monitor;

    public Sample(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void SingleTable_SelectAll()
    {
        var tableA = FluentTable.Create("table_a", ["table_a_id", "value"], "a");

        var sq = new SelectQuery()
            .From(tableA)
            .SelectAll(tableA);

        Monitor.Log(sq);

        var expect = """
            SELECT
                a.table_a_id,
                a.value
            FROM
                table_a AS a
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SingleTable_Equal()
    {
        var tableA = FluentTable.Create("table_a", ["table_a_id", "value"], "a");

        var sq = new SelectQuery()
            .From(tableA)
            .SelectAll(tableA)
            .Equal("table_a_id", 1);

        Monitor.Log(sq);

        var expect = """
            SELECT
                a.table_a_id,
                a.value
            FROM
                table_a AS a
            WHERE
                a.table_a_id = 1
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SingleTable_Select()
    {
        var tableA = FluentTable.Create("table_a", ["table_a_id", "value"], "a");

        var sq = new SelectQuery()
            .From(tableA)
            .Select(tableA, "table_a_id");

        Monitor.Log(sq);

        var expect = """
            SELECT
                a.table_a_id
            FROM
                table_a AS a
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SingleTable_SelectAlias()
    {
        var tableA = FluentTable.Create("table_a", ["table_a_id", "value"], "a");

        var sq = new SelectQuery()
            .From(tableA)
            .Select(tableA, "table_a_id", "id");

        Monitor.Log(sq);

        var expect = """
            SELECT
                a.table_a_id AS id
            FROM
                table_a AS a
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SingleTable_Join()
    {
        var tableA = FluentTable.Create("table_a", ["table_a_id", "value"], "a");
        var tableB = FluentTable.Create("table_b", ["table_a_id", "value"], "b");
        var tableC = FluentTable.Create("table_c", ["table_a_id", "value"], "c");

        var sq = new SelectQuery()
            .From(tableA)
            .InnerJoin(tableB, ["table_a_id"])
            .InnerJoin(tableC, ["table_a_id"]);

        Monitor.Log(sq);

        var expect = """
            SELECT
                *
            FROM
                table_a AS a
                INNER JOIN table_b AS b ON a.table_a_id = b.table_a_id
                INNER JOIN table_c AS c ON a.table_a_id = c.table_a_id
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SingleTable_Join_Strict()
    {
        var tableA = FluentTable.Create("table_a", ["table_a_id", "value"], "a");
        var tableB = FluentTable.Create("table_b", ["table_a_id", "value"], "b");
        var tableC = FluentTable.Create("table_c", ["table_a_id", "value"], "c");

        var sq = new SelectQuery()
            .From(tableA)
            .InnerJoin(tableB, ["table_a_id"], tableA)
            .InnerJoin(tableC, ["table_a_id"], tableB);

        Monitor.Log(sq);

        var expect = """
            SELECT
                *
            FROM
                table_a AS a
                INNER JOIN table_b AS b ON a.table_a_id = b.table_a_id
                INNER JOIN table_c AS c ON b.table_a_id = c.table_a_id
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SingleTable_Join_Custom()
    {
        var tableA = FluentTable.Create("table_a", ["id", "value"], "a");
        var tableB = FluentTable.Create("table_b", ["id", "table_a_id", "value"], "b");
        var tableC = FluentTable.Create("table_c", ["id", "table_a_id", "value"], "c");

        var sq = new SelectQuery()
            .From(tableA)
            .InnerJoin(tableB, ["table_a_id"], tableA, ["id"])
            .InnerJoin(tableC, ["table_a_id"], tableA, ["id"]);

        Monitor.Log(sq);

        var expect = """
            SELECT
                *
            FROM
                table_a AS a
                INNER JOIN table_b AS b ON a.id = b.table_a_id
                INNER JOIN table_c AS c ON a.id = c.table_a_id
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SingleTable_Join_Select()
    {
        var tableA = FluentTable.Create("table_a", ["table_a_id", "value"], "a");
        var tableB = FluentTable.Create("table_b", ["table_a_id", "value"], "b");

        var sq = new SelectQuery()
            .From(tableA)
            .InnerJoin(tableB, ["table_a_id"], tableA)
            .Select(tableA, "table_a_id")
            .Select(tableA, "value", "a_value")
            .Select(tableB, "value", "b_value");

        Monitor.Log(sq);

        var expect = """
            SELECT
                a.table_a_id,
                a.value AS a_value,
                b.value AS b_value
            FROM
                table_a AS a
                INNER JOIN table_b AS b ON a.table_a_id = b.table_a_id
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SingleTable_Join_Equal()
    {
        var tableA = FluentTable.Create("table_a", ["table_a_id", "value"], "a");
        var tableB = FluentTable.Create("table_b", ["table_a_id", "value"], "b");

        var sq = new SelectQuery()
            .From(tableA)
            .InnerJoin(tableB, ["table_a_id"], tableA)
            .Equal("table_a_id", 1);

        Monitor.Log(sq);

        var expect = """
            SELECT
                *
            FROM
                table_a AS a
                INNER JOIN table_b AS b ON a.table_a_id = b.table_a_id
            WHERE
                a.table_a_id = 1
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SingleTable_Join_Equal_Strict()
    {
        var tableA = FluentTable.Create("table_a", ["table_a_id", "value"], "a");
        var tableB = FluentTable.Create("table_b", ["table_a_id", "value"], "b");

        var sq = new SelectQuery()
            .From(tableA)
            .InnerJoin(tableB, ["table_a_id"], tableA)
            .Equal(tableB, "table_a_id", 1);

        Monitor.Log(sq);

        var expect = """
            SELECT
                *
            FROM
                table_a AS a
                INNER JOIN table_b AS b ON a.table_a_id = b.table_a_id
            WHERE
                b.table_a_id = 1
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SingleTable_Exists()
    {
        var tableA = FluentTable.Create("table_a", ["table_a_id", "value"], "a");
        var tableB = FluentTable.Create("table_b", ["table_a_id", "value"], "b");

        var sq = new SelectQuery()
            .From(tableA)
            .ExistsIn(tableB, ["table_a_id"]);

        Monitor.Log(sq);

        var expect = """
            SELECT
                *
            FROM
                table_a AS a
            WHERE
                EXISTS (
                    SELECT
                        *
                    FROM
                        table_b AS b
                    WHERE
                        b.table_a_id = a.table_a_id
                )
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SingleTable_Exists_Strict()
    {
        var tableA = FluentTable.Create("table_a", ["table_a_id", "value"], "a");
        var tableB = FluentTable.Create("table_b", ["table_a_id", "value"], "b");

        var sq = new SelectQuery()
            .From(tableA)
            .ExistsIn(tableB, ["table_a_id"], tableA);

        Monitor.Log(sq);

        var expect = """
            SELECT
                *
            FROM
                table_a AS a
            WHERE
                EXISTS (
                    SELECT
                        *
                    FROM
                        table_b AS b
                    WHERE
                        b.table_a_id = a.table_a_id
                )
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SingleTable_Exists_Custom()
    {
        var tableA = FluentTable.Create("table_a", ["id", "value"], "a");
        var tableB = FluentTable.Create("table_b", ["table_a_id", "value"], "b");

        var sq = new SelectQuery()
            .From(tableA)
            .ExistsIn(tableB, ["table_a_id"], tableA, ["id"]);

        Monitor.Log(sq);

        var expect = """
            SELECT
                *
            FROM
                table_a AS a
            WHERE
                EXISTS (
                    SELECT
                        *
                    FROM
                        table_b AS b
                    WHERE
                        b.table_a_id = a.id
                )
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SingleTable_NotExists_Custom()
    {
        var tableA = FluentTable.Create("table_a", ["id", "value"], "a");
        var tableB = FluentTable.Create("table_b", ["table_a_id", "value"], "b");

        var sq = new SelectQuery()
            .From(tableA)
            .NotExistsIn(tableB, ["table_a_id"], tableA, ["id"]);

        Monitor.Log(sq);

        var expect = """
            SELECT
                *
            FROM
                table_a AS a
            WHERE
                NOT EXISTS (
                    SELECT
                        *
                    FROM
                        table_b AS b
                    WHERE
                        b.table_a_id = a.id
                )
            """;

        Assert.Equal(expect, sq.ToText());
    }
}