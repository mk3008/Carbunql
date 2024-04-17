using Carbunql.Analysis;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class CountTest
{
    private readonly QueryCommandMonitor Monitor;

    public CountTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void CountQuery_With()
    {
        var sql = @"with 
dat as (
    select a.id, a.value as v from table as a
) 
select * from dat";
        var q = QueryParser.Parse(sql);

        var sq = q.ToCountQuery();
        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(35, lst.Count());

    }
    [Fact]
    public void CountQuery()
    {
        var sql = "select a.id, a.value as v from table as a";
        var q = QueryParser.Parse(sql);

        var sq = q.ToCountQuery();
        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(26, lst.Count());
    }

    [Fact]
    public void CountQuery_Values()
    {
        var sql = "values (1, 'a'), (2, 'b')";
        var q = QueryParser.Parse(sql);

        var sq = q.ToCountQuery();
        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(24, lst.Count());
    }
}