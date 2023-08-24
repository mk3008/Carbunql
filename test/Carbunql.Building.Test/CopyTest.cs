using MessagePack;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class CopyTest
{
	private ITestOutputHelper Output;

	public CopyTest(ITestOutputHelper output)
	{
		Output = output;
	}

	private string TruncateControlString(string text)
	{
		return text.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToLower();
	}

	[Fact]
	public void DeepCopy()
	{
		var sq = new SelectQuery("select a.column_1 as c1 from table_a as a");
		var actual = sq.DeepCopy();

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
		Assert.NotEqual(sq, actual);
	}
}
