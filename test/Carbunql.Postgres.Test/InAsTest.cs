using Carbunql.Building;
using Xunit.Abstractions;

namespace Carbunql.Postgres.Test;

public class InAsTest
{
	private readonly QueryCommandMonitor Monitor;

	public InAsTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void Default()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.Where(() => sq.InAs<table_b>(x => a.a_id == x.a_id));

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    table_a AS a
WHERE
    a.a_id IN (
        SELECT
            x.a_id
        FROM
            table_b AS x
    )";

		Assert.Equal(21, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void InAsManyArguments()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.Where(() => sq.InAs<table_b>(x => a.a_id == x.a_id && x.text == a.text));

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    table_a AS a
WHERE
    (a.a_id, a.text) IN (
        SELECT
            x.a_id,
            x.text
        FROM
            table_b AS x
    )";

		Assert.Equal(31, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void NotInAsTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.Where(() => !sq.InAs<table_b>(x => a.a_id == x.a_id));

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    table_a AS a
WHERE
    a.a_id NOT IN (
        SELECT
            x.a_id
        FROM
            table_b AS x
    )";

		Assert.Equal(22, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void InAsTableNameTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.Where(() => sq.InAs<table_b>("TABLE", b => a.a_id == b.a_id));

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    table_a AS a
WHERE
    a.a_id IN (
        SELECT
            b.a_id
        FROM
            TABLE AS b
    )";

		Assert.Equal(21, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void InAsSubQueryTest()
	{
		var subq = new SelectQuery();
		subq.SelectAll();
		var (f, b) = subq.FromAs<table_b>("b");
		subq.Where(() => b.is_enabled);

		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.Where(() => sq.InAs<table_b>(subq, b => a.a_id == b.a_id));

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    table_a AS a
WHERE
    a.a_id IN (
        SELECT
            b.a_id
        FROM
            (
                SELECT
                    *
                FROM
                    table_b AS b
                WHERE
                    b.is_enabled
            ) AS b
    )";

		Assert.Equal(32, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value, bool is_enabled, double rate, DateTime timestamp);

	public record struct table_b(int a_id, int b_id, string text, int value, bool is_enabled, double rate, DateTime timestamp);
}
