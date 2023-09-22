using Xunit.Abstractions;
//using static Carbunql.Building.Test.ExpressionTreeTest;

namespace Carbunql.Building.Test;

public class ExpressionTreeTest
{
	private readonly QueryCommandMonitor Monitor;

	public ExpressionTreeTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void Test()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a");

		sq.SelectAll();

		sq.Where("a_id").Equal(1);
		sq.Where(() => a.a_id == 1);

		Monitor.Log(sq);

		Assert.Equal(18, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void AndTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a");

		sq.SelectAll();

		sq.Where(() => a.a_id == 1 && a.text == "test");

		Monitor.Log(sq);

		Assert.Equal(20, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void OrTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a");

		sq.SelectAll();

		sq.Where(() => a.a_id == 1 || a.text == "test" || a.text == "test2");

		Monitor.Log(sq);

		Assert.Equal(26, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void BracketTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a");

		sq.SelectAll();

		sq.Where(() => (a.a_id == 1 || a.a_id == 2 || a.a_id == 3) && (a.a_id == 3 || a.a_id == 4 || a.a_id == 5));

		Monitor.Log(sq);

		Assert.Equal(48, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void JoinTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a");
		var b = from.InnerJoin("table_b").As<RecordB>("b").On(b => a.a_id == b.a_id && b.text == "test");
		var c = from.LeftJoin("table_c").As<RecordC>("c").On(c => a.a_id == c.a_id);

		sq.SelectAll();

		sq.Where(() => a.a_id == 1 || b.b_id == 2);

		Monitor.Log(sq);

		Assert.Equal(52, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void SelectTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.Select(1);
		sq.Select(() => a.a_id);
		sq.Select(() => a.a_id).As("id");

		Monitor.Log(sq);

		Assert.Equal(18, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void SelectTest_Expression()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.Select("1 + 2 * 3.14");
		sq.Select(() => 1 + 2 * 3.14);
		sq.Select(() => 1 + 2 * 3.14).As("value");

		Monitor.Log(sq);

		Assert.Equal(18, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void SelectTest_Expression_FourArithmeticOperations()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.Select(() => a.value * 2 / 10 + 1 - 3);
		sq.Select(() => a.value * 2 / 10 + 1 - 3).As("value");

		Monitor.Log(sq);

		Assert.Equal(30, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void WhereTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.SelectAll();

		sq.Where(() => true);
		sq.Where(() => false);
		sq.Where(() => 1 <= a.a_id);
		sq.Where(() => a.a_id <= 10);
		sq.Where(() => 0 < a.a_id);
		sq.Where(() => a.a_id < 11);
		sq.Where(() => a.a_id != 5);
		sq.Where(() => a.text == "a");
		sq.Where(() => a.text != "b");

		sq.Where(() => a.is_enablesd == true);
		sq.Where(() => a.is_enablesd != false);

		sq.Where(() => a.timestamp >= new DateTime(2000, 1, 1));

		Monitor.Log(sq);

		Assert.Equal(99, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void WhereTest_VariableDateTime()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.SelectAll();

		var dt = DateTime.Now;
		sq.Where(() => a.timestamp >= dt);

		Monitor.Log(sq);

		Assert.Equal(19, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void WhereTest_DateTimeNow()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.SelectAll();

		sq.Where(() => a.timestamp >= DateTime.Now);

		Monitor.Log(sq);

		Assert.Equal(14, sq.GetTokens().ToList().Count);
	}

	public record struct RecordA(int a_id, string text, int value, bool is_enablesd, DateTime timestamp);

	public record struct RecordB(int a_id, int b_id, string text, int value);

	public record struct RecordC(int a_id, int c_id, string text, int value);
}
