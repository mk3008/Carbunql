using Xunit.Abstractions;

namespace Carbunql.Fluent.Test;

public class FluentTableJoinTest
{
    private readonly QueryCommandMonitor Monitor;

    public FluentTableJoinTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void InnerJoinTest_SubQuery()
    {
        var tableA = FluentTable.Create("select table_a_id from table_a", "a");
        var tableB = FluentTable.Create("select table_a_id from table_b", "b");

        var sq = new SelectQuery()
            .From(tableA)
            .InnerJoin(tableA, tableB, ["table_a_id"])
            .SelectAll(tableA);

        Monitor.Log(sq);

        var expect = """
            SELECT
                a.table_a_id
            FROM
                (
                    SELECT
                        table_a_id
                    FROM
                        table_a
                ) AS a
                INNER JOIN (
                    SELECT
                        table_a_id
                    FROM
                        table_b
                ) AS b ON a.table_a_id = b.table_a_id
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void LeftJoinTest_SubQuery()
    {
        var tableA = FluentTable.Create("select table_a_id from table_a", "a");
        var tableB = FluentTable.Create("select table_a_id from table_b", "b");

        var sq = new SelectQuery()
            .From(tableA)
            .LeftJoin(tableA, tableB, ["table_a_id"])
            .SelectAll(tableA);

        Monitor.Log(sq);

        var expect = """
            SELECT
                a.table_a_id
            FROM
                (
                    SELECT
                        table_a_id
                    FROM
                        table_a
                ) AS a
                LEFT JOIN (
                    SELECT
                        table_a_id
                    FROM
                        table_b
                ) AS b ON a.table_a_id = b.table_a_id
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void RightJoinTest_SubQuery()
    {
        var tableA = FluentTable.Create("select table_a_id from table_a", "a");
        var tableB = FluentTable.Create("select table_a_id from table_b", "b");

        var sq = new SelectQuery()
            .From(tableA)
            .RightJoin(tableA, tableB, ["table_a_id"])
            .SelectAll(tableA);

        Monitor.Log(sq);

        var expect = """
            SELECT
                a.table_a_id
            FROM
                (
                    SELECT
                        table_a_id
                    FROM
                        table_a
                ) AS a
                RIGHT JOIN (
                    SELECT
                        table_a_id
                    FROM
                        table_b
                ) AS b ON a.table_a_id = b.table_a_id
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void InnerJoinTest_Cte()
    {
        var tableA = FluentTable.Create("select table_a_id from table_a", "cte_a", "a");
        var tableB = FluentTable.Create("select table_a_id from table_b", "cte_b", "b");

        var sq = new SelectQuery()
            .From(tableA)
            .InnerJoin(tableA, tableB, ["table_a_id"])
            .SelectAll(tableA);

        Monitor.Log(sq);

        var expect = """
            WITH
                cte_a AS (
                    SELECT
                        table_a_id
                    FROM
                        table_a
                ),
                cte_b AS (
                    SELECT
                        table_a_id
                    FROM
                        table_b
                )
            SELECT
                a.table_a_id
            FROM
                cte_a AS a
                INNER JOIN cte_b AS b ON a.table_a_id = b.table_a_id
            """;

        Assert.Equal(expect, sq.ToText());
    }
}