using Carbunql.Analysis.Parser;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class TableParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public TableParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void PhysicalTable()
    {
        var text = "schema_name.table_name";
        var v = TableParser.Parse(text);
        Monitor.Log(v);

        //Assert.Equal("schema_name.table_name", v.GetCommandText());
        //Assert.Equal("table_name", v.GetDefaultName());
    }

    [Fact]
    public void PhysicalTable2()
    {
        var text = "table_name";
        var v = TableParser.Parse(text);
        Monitor.Log(v);

        //Assert.Equal("table_name", v.GetCommandText());
        //Assert.Equal("table_name", v.GetDefaultName());
    }

    [Fact]
    public void ValuesTable()
    {
        var text = "(values (1,2.3,'a'), (4,5.6,'b'))";
        var v = TableParser.Parse(text);
        Monitor.Log(v);

        var expect =
@"(
    values
        (1, 2.3, 'a'),
        (4, 5.6, 'b')
)".Replace("\r\n", "\n");
        //var actual = v.GetCommandText().Replace("\r\n", "\n");

        //Assert.Equal(expect, actual);
        //Assert.Equal(string.Empty, v.GetDefaultName());
    }
}