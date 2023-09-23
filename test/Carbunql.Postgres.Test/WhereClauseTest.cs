using Carbunql.Building;
using Xunit.Abstractions;

namespace Carbunql.Postgres.Test;

public class WhereClauseTest
{
	private readonly QueryCommandMonitor Monitor;

	public WhereClauseTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
		Output = output;
	}

	private ITestOutputHelper Output { get; set; }

	[Fact]
	public void EqualMinus()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a");

		sq.SelectAll();

		sq.Where(() => a.a_id == -1);

		Monitor.Log(sq);

		Assert.Equal(14, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void Equal()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a");

		sq.SelectAll();

		sq.Where("a_id").Equal(1);
		sq.Where(() => a.a_id == 1);

		Monitor.Log(sq);

		Assert.Equal(18, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void NegativeEqual()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a");

		sq.SelectAll();

		sq.Where(() => !(a.a_id == 1));

		Monitor.Log(sq);

		Assert.Equal(17, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void AndTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a");

		sq.SelectAll();

		sq.Where(() => a.a_id == 1 && a.text == "test");

		Monitor.Log(sq);

		Assert.Equal(20, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void OrTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a");

		sq.SelectAll();

		sq.Where(() => a.a_id == 1 || a.text == "test" || a.text == "test2");

		Monitor.Log(sq);

		Assert.Equal(26, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void BracketTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a");

		sq.SelectAll();

		sq.Where(() => (a.a_id == 1 || a.a_id == 2 || a.a_id == 3) && (a.a_id == 3 || a.a_id == 4 || a.a_id == 5));

		Monitor.Log(sq);

		Assert.Equal(48, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void DefaultTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.SelectAll();

		sq.Where(() => true);
		sq.Where(() => false);

		sq.Where(() => 1 <= a.a_id);
		sq.Where(() => a.a_id <= 10);

		sq.Where(() => 0 < a.a_id);
		sq.Where(() => a.a_id < 11);

		sq.Where(() => a.a_id != 5);

		sq.Where(() => a.text == "a");
		sq.Where(() => a.text != "b");

		sq.Where(() => a.rate == 0.1);

		sq.Where(() => a.is_enabled == true);
		sq.Where(() => a.is_enabled != false);

		sq.Where(() => a.timestamp >= new DateTime(2000, 1, 1));

		Monitor.Log(sq);

		Assert.Equal(107, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void EnumTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.SelectAll();

		sq.Where(() => a.gender == Gender.Male);

		Monitor.Log(sq);

		Assert.Equal(14, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void StringTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;
		sq.SelectAll();

		var text = "';delete";
		Func<string> fn = () => text;

		sq.Where(() => a.text == "abc");
		sq.Where(() => a.text == "';delete");
		sq.Where(() => a.text == text);
		sq.Where(() => a.text == text.Trim());
		sq.Where(() => a.text == fn());

		Monitor.Log(sq);

		var sql = @"
/*
  :member_text = '';delete'
  :invoke_fn = '';delete'
*/
SELECT
    *
FROM
    table_a AS a
WHERE
    (a.text = 'abc')
    AND (a.text = '')
    AND (a.text = :member_text)
    AND (a.text = TRIM(:member_text))
    AND (a.text = :invoke_fn)
";
		Assert.Equal(sql.ToValidateText(), sq.ToText().ToValidateText());
	}

	[Fact]
	public void IntTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.SelectAll();

		int d = 10;
		sq.Where(() => a.a_id == d);

		Monitor.Log(sq);

		Assert.Equal(14, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void DoubleTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.SelectAll();

		double d = 0.4;
		sq.Where(() => a.rate == d);

		Monitor.Log(sq);

		Assert.Equal(14, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void BoolTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.SelectAll();

		bool d = false;
		sq.Where(() => a.is_enabled == d);

		Monitor.Log(sq);

		Assert.Equal(14, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void DateTimeTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.SelectAll();

		var dt = DateTime.Now;
		sq.Where(() => a.timestamp >= dt);

		Monitor.Log(sq);

		Assert.Equal(14, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void DateTimeNowTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;


		sq.Select(() => DateTime.Now);

		sq.Where(() => a.timestamp >= DateTime.Now);

		Monitor.Log(sq);

		Assert.Equal(14, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void NullTest()
	{
		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordN>("a"); ;

		sq.SelectAll();

		sq.Where(() => a.text == null);
		sq.Where(() => null == a.text);
		sq.Where(() => a.text != null);
		sq.Where(() => null != a.text);

		sq.Where(() => a.value == null);
		sq.Where(() => null == a.value);
		sq.Where(() => a.value != null);
		sq.Where(() => null != a.value);

		sq.Where(() => a.is_enabled == null);
		sq.Where(() => null == a.is_enabled);
		sq.Where(() => a.is_enabled != null);
		sq.Where(() => null != a.is_enabled);

		sq.Where(() => a.rate == null);
		sq.Where(() => null == a.rate);
		sq.Where(() => a.rate != null);
		sq.Where(() => null != a.rate);

		sq.Where(() => a.timestamp == null);
		sq.Where(() => null == a.timestamp);
		sq.Where(() => a.timestamp != null);
		sq.Where(() => null != a.timestamp);

		Monitor.Log(sq);

		Assert.Equal(166, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void RecordDefinitionTest()
	{
		var c = new Myclass { MyProperty = 1 };

		var sq = new SelectQuery();
		var (from, a) = sq.From("table_a").As<RecordA>("a"); ;

		sq.SelectAll();

		sq.Where(() => a.a_id == c.MyProperty);

		Monitor.Log(sq);

		Assert.Equal(14, sq.GetTokens().ToList().Count);
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
