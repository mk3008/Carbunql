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
