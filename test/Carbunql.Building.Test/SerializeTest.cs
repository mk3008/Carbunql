using MessagePack;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class SerializeTest
{
	private ITestOutputHelper Output;
	public SerializeTest(ITestOutputHelper output)
	{
		Output = output;
	}

	private string TruncateControlString(string text)
	{
		return text.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToLower();
	}

	[Fact]
	public void ValueQuery()
	{
		var sql = @"select 1";

		var sq = new SelectQuery(sql);
		var json = MessagePackSerializer.Serialize(sq);
		var actual = MessagePackSerializer.Deserialize<SelectQuery>(json);

		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sql), TruncateControlString(actual!.ToText()));
	}
}
