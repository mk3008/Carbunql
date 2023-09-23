using Carbunql.Building;
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
    (a.text LIKE '%' || 'word' || '%')
    AND (a.text LIKE '%' || :member_text || '%')";

		Assert.Equal(30, sq.GetTokens().ToList().Count);
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
    (a.text LIKE 'word' || '%')
    AND (a.text LIKE :member_text || '%')";

		Assert.Equal(26, sq.GetTokens().ToList().Count);
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
    (a.text LIKE '%' || 'word')
    AND (a.text LIKE '%' || :member_text)";

		Assert.Equal(26, sq.GetTokens().ToList().Count);
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[RecordDefinition]
	public record struct table_a(int a_id, string text, int value, bool is_enabled, double rate, DateTime timestamp);
}
