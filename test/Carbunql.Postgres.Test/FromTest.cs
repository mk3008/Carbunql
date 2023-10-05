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
	public void DefaultJoinTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a");

		sq.SelectAll();

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    table_a AS a";

		Assert.Equal(6, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void FromAs()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");

		sq.SelectAll();

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    RecordA AS a";

		Assert.Equal(6, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void FromAs_AttributeTableName()
	{
		var sq = new SelectQuery();
		var (from, b) = sq.FromAs<RecordB>("b");

		sq.SelectAll();

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    table_b AS b";

		Assert.Equal(6, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void FromAs_InputTableName()
	{
		var sq = new SelectQuery();
		var (from, b) = sq.FromAs<RecordB>("INPUT_TABLE", "b");

		sq.SelectAll();

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    INPUT_TABLE AS b";

		Assert.Equal(6, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct RecordA(int a_id, string text, int value, bool is_enabled, double rate, DateTime timestamp, Gender gender);

	public record struct RecordN(int? a_id, string? text, int? value, bool? is_enabled, double? rate, DateTime? timestamp);

	[Table("table_b")]
	public record struct RecordB(int a_id, int b_id, string text, int value);

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