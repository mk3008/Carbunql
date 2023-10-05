using Carbunql.Building;
using Xunit.Abstractions;

namespace Carbunql.Postgres.Test;

public class ExistsTest
{
	private readonly QueryCommandMonitor Monitor;

	public ExistsTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void DefaultTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.WhereAs<table_b>("b").Exists(b => a.a_id == b.a_id);

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
            table_b AS b
        WHERE
            a.a_id = b.a_id
    )";

		Assert.Equal(24, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void NotTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.WhereAs<table_b>("b").NotExists(b => a.a_id == b.a_id);

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
	public void TableNameTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.WhereAs<table_b>("TABLE", "b").Exists(b => a.a_id == b.a_id);

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
	public void SubQueryTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.WhereAs<table_b>(() =>
		{
			var subq = new SelectQuery();
			subq.SelectAll();
			var (f, b) = subq.FromAs<table_b>("b");
			subq.Where(() => b.is_enabled);
			return subq;
		}, "b").Exists(b => a.a_id == b.a_id);

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

	[Fact]
	public void ExistsAsTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.Where(() => sq.ExistsAs<table_b>("b", b => a.a_id == b.a_id));

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
		        table_b AS b
		    WHERE
		        a.a_id = b.a_id
		)";

		Assert.Equal(24, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void OrTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		sq.SelectAll();

		sq.Where(() =>
			sq.ExistsAs<table_b>("b", b => a.a_id == b.a_id)
			|| sq.ExistsAs<table_b>("b", b => a.a_id == b.a_id && a.a_id == 1)
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
            table_b AS b
        WHERE
            a.a_id = b.a_id
    ) OR EXISTS (
        SELECT
            *
        FROM
            table_b AS b
        WHERE
            (a.a_id = b.a_id AND a.a_id = 1)
    ))";

		Assert.Equal(52, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value, bool is_enabled, double rate, DateTime timestamp);

	public record struct table_b(int a_id, int b_id, string text, int value, bool is_enabled, double rate, DateTime timestamp);
}
