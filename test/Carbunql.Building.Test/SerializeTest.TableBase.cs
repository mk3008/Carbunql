using Carbunql.Tables;
using MessagePack;

namespace Carbunql.Building.Test;

public partial class SerializeTest
{
    [Fact]
    public void PhysicalTable()
    {
        var sq = new PhysicalTable("table_a");

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<PhysicalTable>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
    }

    [Fact]
    public void VirtualTable()
    {
        var sq = new VirtualTable(new SelectQuery("select 1"));

        var json = MessagePackSerializer.Serialize(sq);
        Output.WriteLine(MessagePackSerializer.ConvertToJson(json));

        var actual = MessagePackSerializer.Deserialize<VirtualTable>(json);
        Output.WriteLine(actual.ToText());

        Assert.Equal(TruncateControlString(sq.ToText()), TruncateControlString(actual!.ToText()));
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
