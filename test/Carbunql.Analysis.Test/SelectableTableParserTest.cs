using Carbunql.Analysis.Parser;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class SelectableTableParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public SelectableTableParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void PhysicalTable()
    {
        var text = "schema_name.table_name as t";
        var v = SelectableTableParser.Parse(text);
        Monitor.Log(v);

        //Assert.Equal("schema_name.table_name as t", v.GetCommandText());
        //Assert.Equal("t", v.Alias);
    }

    [Fact]
    public void PhysicalTable2()
    {
        var text = "table_name as t";
        var v = SelectableTableParser.Parse(text);
        Monitor.Log(v);

        //Assert.Equal("table_name as t", v.GetCommandText());
        //Assert.Equal("t", v.Alias);
    }


    [Fact]
    public void PhysicalTable3()
    {
        var text = "table_name as table_name";
        var v = SelectableTableParser.Parse(text);
        Monitor.Log(v);

        //Assert.Equal("table_name", v.GetCommandText());
    }

    [Fact]
    public void ValuesTable()
    {
        var text = "(values (1,2.3,'a'), (4,5.6,'b')) as t(a, b, c)";
        var v = SelectableTableParser.Parse(text);
        Monitor.Log(v);

        var expect =
@"(
    values
        (1, 2.3, 'a'),
        (4, 5.6, 'b')
) as t(a, b, c)".Replace("\r\n", "\n");
        //var actual = v.GetCommandText().Replace("\r\n", "\n");

        //Assert.Equal(expect, actual);
    }
}