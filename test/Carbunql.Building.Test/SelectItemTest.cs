using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class SelectItemTest
{
    private readonly QueryCommandMonitor Monitor;

    public SelectItemTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void SelectAllTest_AllTable()
    {
        var sq = new SelectQuery();
        var f = sq.From("table_a");
        sq.SelectAll();

        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(4, lst.Count());

        Assert.Equal("select", lst[0].Text);
        Assert.Equal("*", lst[1].Text);
        Assert.Equal("from", lst[2].Text);
        Assert.Equal("table_a", lst[3].Text);
    }

    [Fact]
    public void SelectAllTest_OneTable()
    {
        var sq = new SelectQuery();
        var f = sq.From("table_a").As("a");
        sq.SelectAll(f.Root);

        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(8, lst.Count());

        Assert.Equal("select", lst[0].Text);
        Assert.Equal("a", lst[1].Text);
        Assert.Equal(".", lst[2].Text);
        Assert.Equal("*", lst[3].Text);
        Assert.Equal("from", lst[4].Text);
        Assert.Equal("table_a", lst[5].Text);
        Assert.Equal("as", lst[6].Text);
        Assert.Equal("a", lst[7].Text);
    }

    [Fact]
    public void ColumnTest()
    {
        var sq = new SelectQuery();
        var f = sq.From("table_a").As("a");

        sq.Select(f.Root, "a_id");
        sq.Select(f.Root, "a_id").As("v1");
        sq.Select(3.14).As("v2");
        sq.Select(new DateTime(2022, 1, 1)).As("v3");
        sq.Select("a.price * a.amount").As("expression_val");

        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(33, lst.Count());

        Assert.Equal("select", lst[0].Text);

        Assert.Equal("a", lst[1].Text);
        Assert.Equal(".", lst[2].Text);
        Assert.Equal("a_id", lst[3].Text);

        Assert.Equal(",", lst[4].Text);

        Assert.Equal("a", lst[5].Text);
        Assert.Equal(".", lst[6].Text);
        Assert.Equal("a_id", lst[7].Text);
        Assert.Equal("as", lst[8].Text);
        Assert.Equal("v1", lst[9].Text);

        Assert.Equal(",", lst[10].Text);

        Assert.Equal("3.14", lst[11].Text);

        Assert.Equal("'2022/01/01 0:00:00'", lst[15].Text);
        Assert.Equal("::timestamp", lst[16].Text);
    }
}