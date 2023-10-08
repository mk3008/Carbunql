using Carbunql.Building;
using Xunit.Abstractions;

namespace Carbunql.Postgres.Test;

public class ExistsAsTest
{
	private readonly QueryCommandMonitor Monitor;

	public ExistsAsTest(ITestOutputHelper output)
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

		sq.Where(() => sq.ExistsAs<table_b>(x => a.a_id == x.a_id));

		Monitor.Log(sq);

		var sql = @"
	SELECT
	    *
	FROM
	    table_a AS a
	WHERE
	    EXISTS (
		    SELECT
		        *
		    FROM
		        table_b AS x
		    WHERE
		        a.a_id = x.a_id
		)";

		Assert.Equal(24, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void ExistsAsOrTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.Where(() =>
			sq.ExistsAs<table_b>(b1 => a.a_id == b1.a_id)
			|| sq.ExistsAs<table_b>(b2 => a.a_id == b2.a_id && a.a_id == 1)
		);

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    table_a AS a
WHERE
    (EXISTS (
        SELECT
            *
        FROM
            table_b AS b1
        WHERE
            a.a_id = b1.a_id
    ) OR EXISTS (
        SELECT
            *
        FROM
            table_b AS b2
        WHERE
            (a.a_id = b2.a_id AND a.a_id = 1)
    ))";

		Assert.Equal(52, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void NotExistsAsTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.Where(() =>
			!sq.ExistsAs<table_b>(b => a.a_id == b.a_id)
		);

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    table_a AS a
WHERE
    NOT EXISTS (
        SELECT
            *
        FROM
            table_b AS b
        WHERE
            a.a_id = b.a_id
    )";

		Assert.Equal(25, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void ExistsAsTableNameTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.Where(() => sq.ExistsAs<table_b>("TABLE", b => a.a_id == b.a_id));

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    table_a AS a
WHERE
    EXISTS (
        SELECT
            *
        FROM
            TABLE AS b
        WHERE
            a.a_id = b.a_id
    )";

		Assert.Equal(24, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void ExistsAsSubQueryTest()
	{
		var subq = new SelectQuery();
		subq.SelectAll();
		var (f, b) = subq.FromAs<table_b>("b");
		subq.Where(() => b.is_enabled);

		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.Where(() => sq.ExistsAs<table_b>(subq, b => a.a_id == b.a_id));

		Monitor.Log(sq);

		var sql = @"
SELECT
    *
FROM
    table_a AS a
WHERE
    EXISTS (
        SELECT
            *
        FROM
            (
                SELECT
                    *
                FROM
                    table_b AS b
                WHERE
                    b.is_enabled
            ) AS b
        WHERE
            a.a_id = b.a_id
    )";

		Assert.Equal(35, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value, bool is_enabled, double rate, DateTime timestamp);

	public record struct table_b(int a_id, int b_id, string text, int value, bool is_enabled, double rate, DateTime timestamp);
}
