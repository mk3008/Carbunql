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
        var tmp = new SelectQuery("with x as (select id, val from table_a as a) select x.id from x");

        //Convert select query to Common table
        //Return value is SelectQuery class and CommonTable class
        var (sq, ct) = tmp.ToCTE("alias");

        //Set common table to From clause
        var t = sq.From(ct);

        //Select all columns of the common table
        sq.Select(t);

        Monitor.Log(sq);

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

        Assert.Equal(30, sq.GetTokens().ToList().Count);
    }
}
