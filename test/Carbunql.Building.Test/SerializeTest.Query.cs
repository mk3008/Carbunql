using MessagePack;

namespace Carbunql.Building.Test;

public partial class SerializeTest
{
	[Fact]
	public void SelectQuery()
	{
		var sq = new SelectQuery("select a.column_1 as c1 from table_a as a");

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<SelectQuery>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void JoinQuery()
	{
		var sq = new SelectQuery("select a.* from table_a as a inner join table_b as b on a.id = b.id");

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<SelectQuery>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
	}
}
