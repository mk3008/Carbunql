using Carbunql.Building;
using Xunit.Abstractions;

namespace Carbunql.Postgres.Test;

public class FromTest
{
	private readonly QueryCommandMonitor Monitor;

	public FromTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

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

	[RecordDefinition]
	public record struct RecordA(int a_id, string text, int value, bool is_enabled, double rate, DateTime timestamp, Gender gender);

	[RecordDefinition]
	public record struct RecordN(int? a_id, string? text, int? value, bool? is_enabled, double? rate, DateTime? timestamp);

	[RecordDefinition]
	public record struct RecordB(int a_id, int b_id, string text, int value);

	[RecordDefinition]
	public record struct RecordC(int a_id, int c_id, string text, int value);

	public class Myclass { public int MyProperty { get; set; } }

	public enum Gender
	{
		Male,
		Female,
		Other,
		Unknown
	}
}