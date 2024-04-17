using Carbunql.Clauses;
using Carbunql.Values;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class WhereTest
{
    private readonly QueryCommandMonitor Monitor;

    public WhereTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var sq = new SelectQuery();
        var (f, a) = sq.From("table_a").As("a");
        sq.SelectAll();

        //*old style
        //
        //sq.Where(() =>
        //{
        //	ValueBase v = new ColumnValue(a, "a_id");
        //	v = v.Equal(new LiteralValue("1"));
        //	return v;
        //});

        sq.Where(a, "a_id").Equal(1);

        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(12, lst.Count());

        Assert.Equal("where", lst[6].Text);
        Assert.Equal("a", lst[7].Text);
        Assert.Equal(".", lst[8].Text);
        Assert.Equal("a_id", lst[9].Text);
        Assert.Equal("=", lst[10].Text);
        Assert.Equal("1", lst[11].Text);
    }

    [Fact]
    public void Grouping()
    {
        var sq = new SelectQuery();
        var (f, a) = sq.From("table_a").As("a");
        sq.SelectAll();

        sq.Where(() =>
        {
            ValueBase v1 = new ColumnValue(a, "a_id");
            v1 = v1.Equal(new LiteralValue("1"));
            v1 = v1.ToGroup();

            ValueBase v2 = new ColumnValue(a, "a_id");
            v2 = v2.Equal(new LiteralValue("2"));
            v2 = v2.ToGroup();

            v1 = v1.Or(v2);

            return v1;
        });

        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(22, lst.Count());

        Assert.Equal("where", lst[6].Text);
        Assert.Equal("(", lst[7].Text);
        Assert.Equal("a", lst[8].Text);
        Assert.Equal(".", lst[9].Text);
        Assert.Equal("a_id", lst[10].Text);
        Assert.Equal("=", lst[11].Text);
        Assert.Equal("1", lst[12].Text);
        Assert.Equal(")", lst[13].Text);
        Assert.Equal("or", lst[14].Text);
        Assert.Equal("(", lst[15].Text);

        Assert.Equal(")", lst[21].Text);
    }

    [Fact]
    public void Parameter()
    {
        var sq = new SelectQuery();
        var (f, a) = sq.From("table_a").As("a");
        sq.SelectAll();

        sq.Where(a, "id").Equal(sq.AddParameter(":val", 1));

        Monitor.Log(sq);

        var lst = sq.GetTokens().ToList();

        Assert.Equal(12, lst.Count());
        Assert.Equal(":val", lst[11].Text);

        Assert.Single(sq.Parameters);
        var val = sq.Parameters.Where(x => x.ParameterName == ":val").Select(x => x.Value).FirstOrDefault();
        if (val == null) throw new NullReferenceException();
        Assert.Equal("1", val.ToString());
    }
}