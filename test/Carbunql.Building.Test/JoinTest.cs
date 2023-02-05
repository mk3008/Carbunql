using Carbunql.Clauses;
using Carbunql.Values;
using Xunit.Abstractions;


namespace Carbunql.Building.Test;

public class JoinTest
{
	private readonly QueryCommandMonitor Monitor;

	public JoinTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void InnerJoinTest()
	{
		var sq = new SelectQuery();
		var (f, a) = sq.From("table_a").As("a");
		var b = f.InnerJoin("table_b").As("b").On(f.Root, "a_id");

		sq.Select(f.Root, "a_id");

		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();

		Assert.Equal(20, lst.Count());

		Assert.Equal("from", lst[4].Text);
		Assert.Equal("table_a", lst[5].Text);
		Assert.Equal("as", lst[6].Text);
		Assert.Equal("a", lst[7].Text);
		Assert.Equal("inner join", lst[8].Text);
		Assert.Equal("table_b", lst[9].Text);
		Assert.Equal("as", lst[10].Text);
		Assert.Equal("b", lst[11].Text);
		Assert.Equal("on", lst[12].Text);
		Assert.Equal("a", lst[13].Text);
		Assert.Equal(".", lst[14].Text);
		Assert.Equal("a_id", lst[15].Text);
		Assert.Equal("=", lst[16].Text);
		Assert.Equal("b", lst[17].Text);
		Assert.Equal(".", lst[18].Text);
		Assert.Equal("a_id", lst[19].Text);
	}

	[Fact]
	public void Sample()
	{
		var sq = new SelectQuery();
		var (f, a) = sq.From("table_a").As("a");
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

	[Fact]
	public void Custom()
	{
		var sq = new SelectQuery();
		var (f, a) = sq.From("table_a").As("a");
		var b = f.InnerJoin("table_b").As("b").On(r =>
		{
			ValueBase v = new ColumnValue(f.Root, "id");
			v = v.Equal(new ColumnValue(r.Table, "a_id"));
			v = v.And(new ColumnValue(r.Table, "value"));
			v = v.Expression(">=", new LiteralValue("10"));
			return v;
		});

		sq.Select(f.Root, "a_id");

		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();

		Assert.Equal(26, lst.Count());

		Assert.Equal("on", lst[12].Text);

		Assert.Equal("a", lst[13].Text);
		Assert.Equal(".", lst[14].Text);
		Assert.Equal("id", lst[15].Text);
		Assert.Equal("=", lst[16].Text);
		Assert.Equal("b", lst[17].Text);
		Assert.Equal(".", lst[18].Text);
		Assert.Equal("a_id", lst[19].Text);

		Assert.Equal("and", lst[20].Text);

		Assert.Equal("b", lst[21].Text);
		Assert.Equal(".", lst[22].Text);
		Assert.Equal("value", lst[23].Text);
		Assert.Equal(">=", lst[24].Text);
		Assert.Equal("10", lst[25].Text);
	}
}