using Xunit.Abstractions;

namespace Carbunql.Fluent.Test;

public class FluentTableTest
{
    private readonly QueryCommandMonitor Monitor;

    public FluentTableTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void PhysicalTableTest()
    {
        var tableA = FluentTable.Create("table_a", "a");

        var sq = new SelectQuery()
            .From(tableA)
            .SelectAll(tableA);

        Monitor.Log(sq);

        var expect = """
            SELECT
                a.*
            FROM
                table_a AS a
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SchemaTest()
    {
        var tableA = FluentTable.Create("public.table_a", "a");

        var sq = new SelectQuery()
            .From(tableA)
            .SelectAll(tableA);

        Monitor.Log(sq);

        var expect = """
            SELECT
                a.*
            FROM
                public.table_a AS a
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void SubQueryTest()
    {
        var tableA = FluentTable.Create("select * from table_a", "a");

        var sq = new SelectQuery()
            .From(tableA)
            .SelectAll(tableA);

        Monitor.Log(sq);

        var expect = """
            SELECT
                a.*
            FROM
                (
                    SELECT
                        *
                    FROM
                        table_a
                ) AS a
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void CteTest()
    {
        var tableA = FluentTable.Create("select * from table_a", "cte_a", "a");

        var sq = new SelectQuery()
            .From(tableA)
            .SelectAll(tableA);

        Monitor.Log(sq);

        var expect = """
            WITH
                cte_a AS (
                    SELECT
                        *
                    FROM
                        table_a
                )
            SELECT
                a.*
            FROM
                cte_a AS a
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void ValuesTest()
    {
        var vals = FluentTable.Create("values (1, 1), (2, 2)", ["id", "value"], "v");

        var sq = new SelectQuery()
            .From(vals)
            .SelectAll(vals);

        Monitor.Log(sq);

        var expect = """
            SELECT
                v.id,
                v.value
            FROM
                (
                    VALUES
                        (1, 1),
                        (2, 2)
                ) AS v (
                    id, value
                )
            """;

        Assert.Equal(expect, sq.ToText());
    }

    [Fact]
    public void ValuesCteTest()
    {
        var vals = FluentTable.Create("values (1, 1), (2, 2)", ["id", "value"], "cte_v", "v");

        var sq = new SelectQuery()
            .From(vals)
            .SelectAll(vals);

        Monitor.Log(sq);

        var expect = """
            WITH
                cte_v (
                    id, value
                ) AS (
                    VALUES
                        (1, 1),
                        (2, 2)
                )
            SELECT
                v.id,
                v.value
            FROM
                cte_v AS v
            """;

        Assert.Equal(expect, sq.ToText());
    }
}