using Carbunql.Building;
using Carbunql.Postgres;
using Xunit.Abstractions;

namespace Carbunql.Postgres.Test;

public class SelectClauseTest
{
	private readonly QueryCommandMonitor Monitor;

	public SelectClauseTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void DefaultTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.Select("a", "a_id");
		sq.Select(nameof(a), "a_id");
		sq.Select(() => a.a_id);
		sq.Select(() => a.a_id).As("id");

		Monitor.Log(sq);

		Assert.Equal(22, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void SelectAll()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.SelectAll(() => a);

		Monitor.Log(sq);

		Assert.Equal(32, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void ExpressionTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.Select("1 + 2 * 3.14");
		sq.Select(() => 1 + 2 * 3.14);
		sq.Select(() => 1 + 2 * 3.14).As("value");
		sq.Select(() => a.value * 2 / 10 + 1 - 3);
		sq.Select(() => a.value * 2 / 10 + 1 - 3).As("value");

		Monitor.Log(sq);

		Assert.Equal(44, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void StringTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		var text = "';delete";
		Func<string> fn = () => text;

		sq.Select(() => "abc ");
		sq.Select(() => "abc ".Trim());
		sq.Select(() => "';delete");
		sq.Select(() => text);
		sq.Select(() => text.Trim());
		sq.Select(() => fn());

		Monitor.Log(sq);

		var sql = @"
/*
  :method_trim = 'abc'
  :member_text = '';delete'
  :method_text_trim = '';delete'
  :invoke_fn = '';delete'
*/
SELECT
    'abc ',
    :method_trim,
    '',
    :member_text,
    :method_text_trim,
    :invoke_fn
FROM
    table_a AS a
";
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
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