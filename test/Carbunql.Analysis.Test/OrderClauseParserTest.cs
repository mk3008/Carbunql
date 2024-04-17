using Carbunql.Analysis.Parser;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class OrderClauseParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public OrderClauseParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = "tbl.col1, 1, tbl.col2 desc nulls first";
        var item = OrderClauseParser.Parse(text);
        Monitor.Log(item);

        Assert.Equal(3, item.Count);
    }
}