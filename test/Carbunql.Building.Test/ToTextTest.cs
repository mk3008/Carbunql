using Carbunql.Analysis;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class ToTextTest
{
	private ITestOutputHelper Output;

	public ToTextTest(ITestOutputHelper output)
	{
		Output = output;
	}

	private string TruncateControlString(string text)
	{
		return text.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToLower();
	}

	[Fact]
	public void ToText()
	{
		var sql = @"
/*
  :id = 1
  :value = 'test'
*/
select a.column_1 as c1 from table_a as a";
		var sq = new SelectQuery(sql);
		sq.AddParameter(":id", 1);
		sq.AddParameter(":value", "test");

		Output.WriteLine(sq.ToText());

		Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToText()));
	}

	[Fact]
	public void ToOneLineText()
	{
		var sql = @"
/*
  :id = 1
  :value = 'test'
*/
select a.column_1 as c1 from table_a as a";
		var sq = new SelectQuery(sql);
		sq.AddParameter(":id", 1);
		sq.AddParameter(":value", "test");

		Output.WriteLine(sq.ToOneLineText());

		Assert.Equal(TruncateControlString(sql), TruncateControlString(sq.ToOneLineText()));
	}
}
