using Carbunql.Analysis.Parser;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class SortableItemParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public SortableItemParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = "a.id";
        var item = SortableItemParser.Parse(text);
        Monitor.Log(item);

        //Assert.Equal("a.id", item.GetCommandText());
    }

    [Fact]
    public void Asc()
    {
        var text = "a.id asc";
        var item = SortableItemParser.Parse(text);
        Monitor.Log(item);

        //Assert.Equal("a.id", item.GetCommandText());
    }

    [Fact]
    public void AscNullSort()
    {
        var text = "a.id asc nulls first";
        var item = SortableItemParser.Parse(text);
        Monitor.Log(item);

        //Assert.Equal("a.id nulls first", item.GetCommandText());
    }

    [Fact]
    public void Desc()
    {
        var text = "a.id desc";
        var item = SortableItemParser.Parse(text);
        Monitor.Log(item);

        //Assert.Equal("a.id desc", item.GetCommandText());
    }

    [Fact]
    public void DescNullSort()
    {
        var text = "a.id desc nulls last";
        var item = SortableItemParser.Parse(text);
        Monitor.Log(item);

        //Assert.Equal("a.id desc nulls last", item.GetCommandText());
    }
}