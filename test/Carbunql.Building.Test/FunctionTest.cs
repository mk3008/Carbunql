using Carbunql.Clauses;
using Carbunql.Values;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class FunctionTest
{
    private readonly QueryCommandMonitor Monitor;

    public FunctionTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void ConcatTest()
    {
        var sq = new SelectQuery();
        var f = sq.From("table_a").As("a");
        sq.Select(() =>
        {
            var v = new ValueCollection();
            v.Add(new LiteralValue("'a'"));
            v.Add(new LiteralValue("'b'"));
            v.Add(new LiteralValue("'c'"));
            return new FunctionValue("concat", v);
        }).As("val");

        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(15, lst.Count());

        Assert.Equal("concat", lst[1].Text);
        Assert.Equal("(", lst[2].Text);
        Assert.Equal("'a'", lst[3].Text);
        Assert.Equal(",", lst[4].Text);
        Assert.Equal(")", lst[8].Text);
    }

    [Fact]
    public void RowNumberTest()
    {
        var sq = new SelectQuery();
        var (f, a) = sq.From("table_a").As("a");
        sq.Select(() =>
        {
            var wf = new OverClause();
            wf.Partition(new ColumnValue(a, "parent_id"));
            wf.Order(new ColumnValue(a, "id").ToSortable());
            return new FunctionValue("row_number", wf);
        }).As("val");

        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(21, lst.Count());

        Assert.Equal("row_number", lst[1].Text);
        Assert.Equal("(", lst[2].Text);
        Assert.Equal(")", lst[3].Text);
        Assert.Equal("over", lst[4].Text);
        Assert.Equal("(", lst[5].Text);
        Assert.Equal("partition by", lst[6].Text);
        Assert.Equal("a", lst[7].Text);
        Assert.Equal(".", lst[8].Text);
        Assert.Equal("parent_id", lst[9].Text);
        Assert.Equal("order by", lst[10].Text);
        Assert.Equal("a", lst[11].Text);
        Assert.Equal(".", lst[12].Text);
        Assert.Equal("id", lst[13].Text);
        Assert.Equal(")", lst[14].Text);
    }

    [Fact]
    public void CoalesceTest()
    {
        var sq = new SelectQuery();
        var f = sq.From("table_a").As("a");
        sq.Select(() =>
        {
            var v = new ValueCollection();
            v.Add(new LiteralValue("'a'"));
            v.Add(new LiteralValue("'b'"));
            v.Add(new LiteralValue("'c'"));
            return new FunctionValue("coalesce", v);
        }).As("val");

        Monitor.Log(sq);

        var sql = @"SELECT
    COALESCE('a', 'b', 'c') AS val
FROM
    table_a AS a";

        Assert.Equal(sql, sq.ToText(), true, true, true);
    }

    [Fact]
    public void CoalesceTest_ValueCollection()
    {
        var sq = new SelectQuery();
        var (f, a) = sq.From("table_a").As("a");

        var args = new ValueCollection
        {
            "'a'",
            1,
            3.14,
            { a, "column1" }
        };

        sq.Select(new FunctionValue("coalesce", args)).As("val");

        Monitor.Log(sq);

        var sql = @"SELECT
    COALESCE('a', 1, 3.14, a.column1) AS val
FROM
    table_a AS a";

        Assert.Equal(sql, sq.ToText(), true, true, true);
    }

    [Fact]
    public void CoalesceTest_Params()
    {

        var sq = new SelectQuery();
        var (f, a) = sq.From("table_a").As("a");

        ValueBase[] args = {
            new LiteralValue("'a'"),
            new LiteralValue("1"),
            new LiteralValue("3.14"),
            new ColumnValue(a, "column1")
        };

        sq.Select(new FunctionValue("coalesce", args)).As("val");

        Monitor.Log(sq);

        var sql = @"SELECT
    COALESCE('a', 1, 3.14, a.column1) AS val
FROM
    table_a AS a";

        Assert.Equal(sql, sq.ToText(), true, true, true);
    }
}