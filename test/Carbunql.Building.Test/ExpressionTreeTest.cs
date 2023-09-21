using Carbunql.Clauses;
using Xunit.Abstractions;
using static Carbunql.Building.Test.ExpressionTreeTest;

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
		var (from, a) = sq.From("table_a").As<RowA>("a");

		sq.SelectAll();

		sq.Where("a_id").Equal(1);
		sq.Where(() => a.a_id == 1);

		Monitor.Log(sq);
	}

	[Fact]
	public void AndTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RowA>("a");

		sq.SelectAll();

		sq.Where(() => a.a_id == 1 && a.value == "test");

		Monitor.Log(sq);
	}

	[Fact]
	public void OrTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RowA>("a");

		sq.SelectAll();

		sq.Where(() => a.a_id == 1 || a.value == "test" || a.value == "test2");

		Monitor.Log(sq);
	}

	[Fact]
	public void BracketTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RowA>("a");

		sq.SelectAll();

		sq.Where(() => (a.a_id == 1 || a.a_id == 2 || a.a_id == 3) && (a.a_id == 3 || a.a_id == 4 || a.a_id == 5));

		Monitor.Log(sq);
	}

	public struct RowA
	{
		public int a_id { get; }
		public string value { get; }
	}
}
