using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class SelectQueryTest
{
    private readonly QueryCommandMonitor Monitor;

    public SelectQueryTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
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

    [Fact]
    public void JoinTest()
    {
        var sq = new SelectQuery();
        var f = sq.From("table_a").As("a");
        var b = f.InnerJoin("table_b").As("b").On(f.Root, "a_id");
        var c = f.InnerJoin("table_c").As("c").On(b, new[] { "b_id", "b_sub_id" });
        var d = f.LeftJoin("table_d").As("d").On(f.Root, "a_id");
        var e = f.RightJoin("table_e").As("e").On(f.Root, "a_id");
        var x = f.CrossJoin("table_x").As("x");

        sq.Select(f.Root, "a_id");

        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(68, lst.Count());

        Assert.Equal("from", lst[4].Text);
        Assert.Equal("inner join", lst[8].Text);
        Assert.Equal("inner join", lst[20].Text);
        Assert.Equal("left join", lst[40].Text);
        Assert.Equal("right join", lst[52].Text);
        Assert.Equal("cross join", lst[64].Text);
    }
}