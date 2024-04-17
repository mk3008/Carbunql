using Carbunql.Analysis.Parser;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class WithClauseParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public WithClauseParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = "with cte1 as (select id from table_a), cte2 as (select id from table_b)";
        var item = WithClauseParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(18, lst.Count);
    }

    [Fact]
    public void Recursive()
    {
        var text = "with recursive cte1 as (select id from table_a), cte2 as (select id from table_b)";
        var item = WithClauseParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(18, lst.Count);
    }

    [Fact]
    public void Materialized()
    {
        var text = "with cte1 as materialized (select id from table_a), cte2 as not materialized (select id from table_b)";
        var item = WithClauseParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(20, lst.Count);
    }

    [Fact]
    public void Comment()
    {
        var text = @"with
a as (
--comment
select 1
)";
        var item = WithClauseParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(7, lst.Count);
    }
}