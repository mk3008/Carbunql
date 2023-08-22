using Carbunql.Tables;
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
		var sql = @"select 1, 2 as v2";

		var sq = new SelectQuery(sql);
		
		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<SelectQuery>(json);
		Output.WriteLine(actual.ToText());

		Assert.Equal(TruncateControlString(sql), TruncateControlString(actual!.ToText()));
	}

	[Fact]
	public void PhysicalTable()
	{
		var sq = new PhysicalTable("table_a");

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json)); 
		
		var actual = MessagePackSerializer.Deserialize<PhysicalTable>(json);
		Output.WriteLine(actual.ToText());
	}

	[Fact]
	public void VirtualTable()
	{
		var sq = new VirtualTable(new SelectQuery("select 1"));

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<VirtualTable>(json);
		Output.WriteLine(actual.ToText());
	}

	[Fact]
	public void FunctionTable()
	{
		var sq = new FunctionTable("fn_table");

		var json = MessagePackSerializer.Serialize(sq);
		Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

		var actual = MessagePackSerializer.Deserialize<FunctionTable>(json);
		Output.WriteLine(actual.ToText());
	}
}
