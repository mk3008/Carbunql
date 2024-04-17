using Carbunql.Analysis.Parser;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class HavingClauseParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public HavingClauseParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = "sum(tbl.col1) = 1 and (sum(tbl.col2) = 2 or sum(tbl.col3) = 3)";
        var item = HavingClauseParser.Parse(text);
        Monitor.Log(item);
    }
}
