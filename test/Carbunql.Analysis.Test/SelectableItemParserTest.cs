using Carbunql.Analysis.Parser;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class SelectableItemParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public SelectableItemParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void NotTableColumn()
    {
        var text = "3.14";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        //Assert.Equal("3.14", item.GetCommandText());
        //Assert.Equal("", item.Alias);
    }

    [Fact]
    public void NotTableColumnAlias()
    {
        var text = "3.14 as val";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        //Assert.Equal("3.14 as val", item.GetCommandText());
        //Assert.Equal("val", item.Alias);
    }

    [Fact]
    public void TableColumn()
    {
        var text = "t.col";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        //Assert.Equal("t.col", item.GetCommandText());
        //Assert.Equal("col", item.Alias);
    }

    [Fact]
    public void TableColumnAliasRedundant()
    {
        var text = "t.col as col";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        //Assert.Equal("t.col", item.GetCommandText());
        //Assert.Equal("col", item.Alias);
    }

    [Fact]
    public void TableColumnAlias()
    {
        var text = "t.col as col1";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        //Assert.Equal("t.col as col1", item.GetCommandText());
        //Assert.Equal("col1", item.Alias);
    }

    [Fact]
    public void TableColumnAlias1()
    {
        var text = "t.col col1";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        //Assert.Equal("t.col as col1", item.GetCommandText());
        //Assert.Equal("col1", item.Alias);
    }

    [Fact]
    public void BreakToken()
    {
        var text = "t.col ,";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        //Assert.Equal("t.col", item.GetCommandText());
        //Assert.Equal("col", item.Alias);
    }
}