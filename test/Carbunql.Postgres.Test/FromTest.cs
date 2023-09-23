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
		var b = from.InnerJoin("table_b").As<RecordB>("b").On(b => a.a_id == b.a_id && b.text == "test");
		var c = from.LeftJoin("table_c").As<RecordC>("c").On(c => a.a_id == c.a_id);

		sq.SelectAll();

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    table_a AS a
    INNER JOIN table_b AS b ON (a.a_id = b.a_id AND b.text = 'test')
    LEFT JOIN table_c AS c ON a.a_id = c.a_id";

		Assert.Equal(38, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void FromAs()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");

		sq.SelectAll(() => a);

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value,
    a.is_enabled,
    a.rate,
    a.timestamp,
    a.gender
FROM
    RecordA AS a";

		Assert.Equal(32, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void FromAs_AttributeTableName()
	{
		var sq = new SelectQuery();
		var (from, b) = sq.FromAs<RecordB>("b");

		sq.SelectAll(() => b);

		Monitor.Log(sq);

		var sql = @"
SELECT
    b.a_id,
    b.b_id,
    b.text,
    b.value
FROM
    table_b AS b";

		Assert.Equal(20, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void FromAs_InputTableName()
	{
		var sq = new SelectQuery();
		var (from, b) = sq.FromAs<RecordB>("INPUT_TABLE", "b");

		sq.SelectAll(() => b);

		Monitor.Log(sq);

		var sql = @"
SELECT
    b.a_id,
    b.b_id,
    b.text,
    b.value
FROM
    INPUT_TABLE AS b";

		Assert.Equal(20, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void InnerJoinAs()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");
		var b = from.InnerJoinAs<RecordB>("b").On(b => a.a_id == b.a_id);

		sq.SelectAll(() => a);

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value,
    a.is_enabled,
    a.rate,
    a.timestamp,
    a.gender
FROM
    RecordA AS a
    INNER JOIN table_b AS b ON a.a_id = b.a_id";

		Assert.Equal(44, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void InnerJoinAs_InputTable()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");
		var b = from.InnerJoinAs<RecordB>("INPUT_TABLE", "b").On(b => a.a_id == b.a_id);

		sq.SelectAll(() => a);

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value,
    a.is_enabled,
    a.rate,
    a.timestamp,
    a.gender
FROM
    RecordA AS a
    INNER JOIN INPUT_TABLE AS b ON a.a_id = b.a_id";

		Assert.Equal(44, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void LeftJoinAs()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");
		var b = from.LeftJoinAs<RecordB>("b").On(b => a.a_id == b.a_id);

		sq.SelectAll(() => a);

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value,
    a.is_enabled,
    a.rate,
    a.timestamp,
    a.gender
FROM
    RecordA AS a
    LEFT JOIN table_b AS b ON a.a_id = b.a_id";

		Assert.Equal(44, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void RightJoinAs()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");
		var b = from.RightJoinAs<RecordB>("b").On(b => a.a_id == b.a_id);

		sq.SelectAll(() => a);

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value,
    a.is_enabled,
    a.rate,
    a.timestamp,
    a.gender
FROM
    RecordA AS a
    RIGHT JOIN table_b AS b ON a.a_id = b.a_id";

		Assert.Equal(44, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void CrossJoinAs()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");
		var b = from.CrossJoinAs<RecordB>("b");

		sq.SelectAll(() => a);

		Monitor.Log(sq);

		var sql = @"
SELECT
    a.a_id,
    a.text,
    a.value,
    a.is_enabled,
    a.rate,
    a.timestamp,
    a.gender
FROM
    RecordA AS a
    CROSS JOIN table_b AS b";

		Assert.Equal(36, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[RecordDefinition]
	public record struct RecordA(int a_id, string text, int value, bool is_enabled, double rate, DateTime timestamp, Gender gender);

	[RecordDefinition]
	public record struct RecordN(int? a_id, string? text, int? value, bool? is_enabled, double? rate, DateTime? timestamp);

	[RecordDefinition("table_b")]
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