using Carbunql.Building;
using Xunit.Abstractions;

namespace Carbunql.Postgres.Test;

public class JoinTest
{
	private readonly QueryCommandMonitor Monitor;

	public JoinTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void JoinAsTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");
		var b = from.InnerJoinAs<RecordB>("b").On(b => a.a_id == b.a_id && b.text == "test");
		var c = from.LeftJoinAs<RecordC>("c").On(c => a.a_id == c.a_id);
		var d = from.RightJoinAs<RecordC>("d").On(d => a.a_id == d.a_id);
		var e = from.CrossJoinAs<RecordC>("e");

		sq.SelectAll();

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    RecordA AS a
    INNER JOIN table_b AS b ON (a.a_id = b.a_id AND b.text = 'test')
    LEFT JOIN RecordC AS c ON a.a_id = c.a_id
    RIGHT JOIN RecordC AS d ON a.a_id = d.a_id
    CROSS JOIN RecordC AS e";

		Assert.Equal(54, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void InnerJoinAs()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");
		var b = from.InnerJoinAs<RecordB>("b").On(b => a.a_id == b.a_id);

		sq.SelectAll(() => b);

		Monitor.Log(sq);

		var sql = @"
SELECT
    b.a_id,
    b.b_id,
    b.text,
    b.value
FROM
    RecordA AS a
    INNER JOIN table_b AS b ON a.a_id = b.a_id";

		Assert.Equal(32, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void InputTable()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");
		var b = from.InnerJoinAs<RecordB>("INPUT_TABLE", "b").On(b => a.a_id == b.a_id);

		sq.SelectAll(() => b);

		Monitor.Log(sq);

		var sql = @"
SELECT
    b.a_id,
    b.b_id,
    b.text,
    b.value
FROM
    RecordA AS a
    INNER JOIN INPUT_TABLE AS b ON a.a_id = b.a_id";

		Assert.Equal(32, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void SubQuery()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");
		var b = from.InnerJoinAs<RecordB>(() =>
		{
			var subq = new SelectQuery();
			var (from, b) = subq.FromAs<RecordB>("b");
			subq.SelectAll();
			subq.Where(() => b.b_id <= 10);
			return subq;
		}, "b").On(b => a.a_id == b.a_id);

		sq.SelectAll(() => b);

		Monitor.Log(sq);

		var sql = @"
SELECT
    b.a_id,
    b.b_id,
    b.text,
    b.value
FROM
    RecordA AS a
    INNER JOIN (
        SELECT
            *
        FROM
            table_b AS b
        WHERE
            (b.b_id <= 10)
    ) AS b ON a.a_id = b.a_id";

		Assert.Equal(47, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void LeftJoinAs()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");
		var b = from.LeftJoinAs<RecordB>("b").On(b => a.a_id == b.a_id);

		sq.SelectAll(() => b);

		Monitor.Log(sq);

		var sql = @"
SELECT
    b.a_id,
    b.b_id,
    b.text,
    b.value
FROM
    RecordA AS a
    LEFT JOIN table_b AS b ON a.a_id = b.a_id";

		Assert.Equal(32, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void RightJoinAs()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");
		var b = from.RightJoinAs<RecordB>("b").On(b => a.a_id == b.a_id);

		sq.SelectAll(() => b);

		Monitor.Log(sq);

		var sql = @"
SELECT
    b.a_id,
    b.b_id,
    b.text,
    b.value
FROM
    RecordA AS a
    RIGHT JOIN table_b AS b ON a.a_id = b.a_id";

		Assert.Equal(32, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void CrossJoinAs()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<RecordA>("a");
		var b = from.CrossJoinAs<RecordB>("b");

		sq.SelectAll(() => b);

		Monitor.Log(sq);

		var sql = @"
SELECT
    b.a_id,
    b.b_id,
    b.text,
    b.value
FROM
    RecordA AS a
    CROSS JOIN table_b AS b";

		Assert.Equal(24, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct RecordA(int a_id, string text, int value, bool is_enabled, double rate, DateTime timestamp, Gender gender);

	[Table("table_b")]
	public record struct RecordB(int a_id, int b_id, string text, int value);

	public record struct RecordC(int a_id, int c_id, string text, int value, bool is_enabled);

	public enum Gender
	{
		Male,
		Female,
		Other,
		Unknown
	}
}