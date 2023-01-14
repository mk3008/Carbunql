using Carbunql.Analysis;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class InsertTest
{
    private readonly QueryCommandMonitor Monitor;

    public InsertTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void InsertQuery()
    {
        var sql = "select a.id, a.value as v from table as a";
        var q = QueryParser.Parse(sql);

        var iq = q.ToInsertQuery("new_table");
        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(21, lst.Count());
    }

    [Fact]
    public void InsertQuery_Values()
    {
        var sql = "values (1, 'a'), (2, 'b')";
        var q = QueryParser.Parse(sql);

        var iq = q.ToInsertQuery("new_table");
        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(14, lst.Count());
    }

    [Fact]
    public void InsertQuery_ColumnFilter()
    {
        var sql = "select a.id, a.value as v from table as a";
        var q = QueryParser.Parse(sql);

        q = q.ToSubQuery("q", (x) =>
        {
            if (x.Alias == "id") return false;
            return true;
        });

        var iq = q.ToInsertQuery("new_table");
        Monitor.Log(iq);

        var lst = iq.GetTokens().ToList();

        Assert.Equal(28, lst.Count());
    }
}