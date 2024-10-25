using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class ToCTETest
{
    private readonly QueryCommandMonitor Monitor;

    public ToCTETest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Old()
    {
        var tmp = SelectQuery.Parse("with x as (select id, val from table_a as a) select x.id from x");

        //Convert select query to Common table
        //Return value is SelectQuery class and CommonTable class
        var (sq, ct) = tmp.ToCTE("alias");

        //Set common table to From clause
        var t = sq.From(ct);

        //Select all columns of the common table
        sq.Select(t);

        Monitor.Log(sq);

        var expect = """
            WITH
                x AS (
                    SELECT
                        id,
                        val
                    FROM
                        table_a AS a
                ),
                alias AS (
                    SELECT
                        x.id
                    FROM
                        x
                )
            SELECT
                alias.id
            FROM
                alias
            """;

        Assert.Equal(expect, sq.ToText());

        Assert.Equal(30, sq.GetTokens().ToList().Count);
    }

    [Fact]
    public void Renew()
    {
        var sq = new SelectQuery();
        var a = sq.With("with x as (select id, val from table_a as a) select x.id from x").As("alias");

        var f = sq.From(a);
        sq.Select(f);

        Monitor.Log(sq);

        var expect = """
            WITH
                x AS (
                    SELECT
                        id,
                        val
                    FROM
                        table_a AS a
                ),
                alias AS (
                    SELECT
                        x.id
                    FROM
                        x
                )
            SELECT
                alias.id
            FROM
                alias
            """;

        Assert.Equal(expect, sq.ToText());

        Assert.Equal(30, sq.GetTokens().ToList().Count);
    }
}
