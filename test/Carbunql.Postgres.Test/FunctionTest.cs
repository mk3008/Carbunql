using Carbunql.Building;
using Carbunql.Clauses;
using Carbunql.Values;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace Carbunql.Postgres.Test;

public class FunctionTest
{
	private readonly QueryCommandMonitor Monitor;

	public FunctionTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void AnyTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");

		var list = new List<int>() { 1, 2, 3 };
		var array = new int[] { 1, 2, 3 };

		sq.SelectAll();

		sq.Where(() => list.Contains(a.a_id));
		sq.Where(() => !list.Contains(a.a_id));

		sq.Where(() => array.Contains(a.a_id));
		sq.Where(() => !array.Contains(a.a_id));

		Monitor.Log(sq);

		var sql = @"
/*
  :member_list = System.Collections.Generic.List`1[System.Int32]
  :member_array = System.Int32[]
*/
SELECT
    *
FROM
    table_a AS a
WHERE
    a.a_id = ANY(:member_list)
    AND NOT (a.a_id = ANY(:member_list))
    AND a.a_id = ANY(:member_array)
    AND NOT (a.a_id = ANY(:member_array))";

		Assert.Equal(48, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void ConcatTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");

		sq.Select(() => string.Concat(a.a_id, " ", a.text));
		sq.Select(() => a.a_id + " " + a.text);

		Monitor.Log(sq);

		var sql = @"
SELECT
    CONCAT(a.a_id, ' ', a.text),
    a.a_id || ' ' || a.text
FROM
    table_a AS a";

		Assert.Equal(27, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void TrimTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");

		sq.Select(() => a.text.Trim());
		sq.Select(() => " value ".Trim());

		Monitor.Log(sq);

		var sql = @"
/*
  :method_trim = 'value'
*/
SELECT
    TRIM(a.text),
    :method_trim
FROM
    table_a AS a";

		Assert.Equal(13, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void TrimStartTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");

		sq.Select(() => a.text.TrimStart());
		sq.Select(() => " value ".TrimStart());

		Monitor.Log(sq);

		var sql = @"
/*
  :method_trim_start = 'value '
*/
SELECT
    LTRIM(a.text),
    :method_trim_start
FROM
    table_a AS a";

		Assert.Equal(13, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void TrimEndTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");

		sq.Select(() => a.text.TrimEnd());
		sq.Select(() => " value ".TrimEnd());

		Monitor.Log(sq);

		var sql = @"
/*
  :method_trim_end = ' value'
*/
SELECT
    RTRIM(a.text),
    :method_trim_end
FROM
    table_a AS a";

		Assert.Equal(13, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void LikeTest_Contains()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");

		var text = "word";

		sq.SelectAll();

		sq.Where(() => a.text.Contains("word"));
		sq.Where(() => a.text.Contains(text));

		Monitor.Log(sq);

		var sql = @"
/*
  :member_text = 'word'
*/
SELECT
    *
FROM
    table_a AS a
WHERE
    a.text LIKE '%' || 'word' || '%'
    AND a.text LIKE '%' || :member_text || '%'";

		Assert.Equal(26, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void LikeTest_NotContains()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");

		var text = "word";

		sq.SelectAll();

		sq.Where(() => !a.text.Contains("word"));
		sq.Where(() => !a.text.Contains(text));

		Monitor.Log(sq);

		var sql = @"
/*
  :member_text = 'word'
*/
SELECT
    *
FROM
    table_a AS a
WHERE
    a.text NOT LIKE '%' || 'word' || '%'
    AND a.text NOT LIKE '%' || :member_text || '%'";

		Assert.Equal(28, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void LikeTest_StartsWith()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");

		var text = "word";

		sq.SelectAll();

		sq.Where(() => a.text.StartsWith("word"));
		sq.Where(() => a.text.StartsWith(text));

		Monitor.Log(sq);

		var sql = @"
/*
  :member_text = 'word'
*/
SELECT
    *
FROM
    table_a AS a
WHERE
    a.text LIKE 'word' || '%'
    AND a.text LIKE :member_text || '%'";

		Assert.Equal(22, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void LikeTest_EndsWith()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");

		var text = "word";

		sq.SelectAll();

		sq.Where(() => a.text.EndsWith("word"));
		sq.Where(() => a.text.EndsWith(text));

		Monitor.Log(sq);

		var sql = @"
/*
  :member_text = 'word'
*/
SELECT
    *
FROM
    table_a AS a
WHERE
    a.text LIKE '%' || 'word'
    AND a.text LIKE '%' || :member_text";

		Assert.Equal(22, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}


	[Fact]
	public void GreatestTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		var b = from.InnerJoinAs<table_b>(b => a.a_id == b.a_id);

		var args = new ValueBase[]
		{
			new LiteralValue(1),
			sq.GetColumn(() => a.a_id),
			sq.GetColumn(() => b.a_id)
		};

		sq.Select(() => sq.Greatest(() => args)).As("max_value");

		Monitor.Log(sq);

		var sql = @"
SELECT
    GREATEST(1, a.a_id, b.a_id) AS max_value
FROM
    table_a AS a
    INNER JOIN table_b AS b ON a.a_id = b.a_id";

		Assert.Equal(31, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void LeastTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.FromAs<table_a>("a");
		var b = from.InnerJoinAs<table_b>(b => a.a_id == b.a_id);

		var args = new ValueBase[]
		{
			new LiteralValue(1),
			sq.GetColumn(() => a.a_id),
			sq.GetColumn(() => b.a_id)
		};

		sq.Select(() => sq.Least(() => args)).As("min_value");

		Monitor.Log(sq);

		var sql = @"
SELECT
    LEAST(1, a.a_id, b.a_id) AS min_value
FROM
    table_a AS a
    INNER JOIN table_b AS b ON a.a_id = b.a_id";

		Assert.Equal(31, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	public record struct table_a(int a_id, string text, int value, bool is_enabled, double rate, DateTime timestamp);

	public record struct table_b(int a_id, string text, int value, bool is_enabled, double rate, DateTime timestamp);
}
