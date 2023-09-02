using Carbunql.Analysis;
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

	[Fact]
	public void Serialize_SelectQuery()
	{
		var sq = new SelectQuery("select a.column_1 as c1 from table_a as a");
		var json = Serializer.Serialize(sq);
		var actual = Serializer.Deserialize<SelectQuery>(json);

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
		Assert.NotEqual(sq, actual);
	}

	[Fact]
	public void Serialize()
	{
		var sq = new SelectQuery("select a.column_1 as c1 from table_a as a");
		var json = Serializer.Serialize(sq);
		var actual = Serializer.Deserialize(json);

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
		Assert.NotEqual(sq, actual);
	}

	//[Fact]
	//public void DeserializeFromByte()
	//{
	//	var sq = new SelectQuery("select a.column_1 as c1 from table_a as a");
	//	var json = new byte[] { 146, 0, 154, 144, 145, 146, 146, 6, 147, 192, 161, 97, 168, 99, 111, 108, 117, 109, 110, 95, 49, 162, 99, 49, 146, 147, 146, 1, 148, 194, 192, 167, 116, 97, 98, 108, 101, 95, 97, 192, 161, 97, 192, 192, 192, 192, 192, 192, 192, 192, 128 };
	//	var actual = Serializer.Deserialize(json);

	//	Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	//	Assert.NotEqual(sq, actual);
	//}
}
