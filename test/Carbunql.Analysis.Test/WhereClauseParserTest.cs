using Carbunql.Analysis.Parser;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class WhereClauseParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public WhereClauseParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = @"tbl.col1 = 1 
and (tbl.col2 = 2 or tbl.col3 = 3) 
and (tbl.col2 = 2 and tbl.col3 = 3) 
and t1.c1 between 1 and 10";
        var item = WhereClauseParser.Parse(text);
        Monitor.Log(item);
    }
}
