using Carbunql.Analysis.Parser;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class SelectClauseParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public SelectClauseParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = "tbl.col1 as col1, tbl.col1 as c1, tbl.col2 c2, tbl.col3, 3.14 as val, 1.23, (tbl.col1 + tbl.col2) / tbl.col3 as colcalc, (1+2)*3 as numcalc";
        var item = SelectClauseParser.Parse(text);
        Monitor.Log(item);

        Assert.False(item.HasDistinctKeyword);
        Assert.Equal(8, item.Count);
    }

    [Fact]
    public void Distinct()
    {
        var text = "distinct tbl.col1 as col1, tbl.col1 as c1, tbl.col2 c2, tbl.col3, 3.14 as val, 1.23, (tbl.col1 + tbl.col2) / tbl.col3 as colcalc, (1+2)*3 as numcalc";
        var item = SelectClauseParser.Parse(text);
        Monitor.Log(item);

        Assert.True(item.HasDistinctKeyword);
        Assert.Equal(8, item.Count);
    }

    [Fact]
    public void DistinctTop()
    {
        var text = "distinct top 10 tbl.col1 as col1, tbl.col1 as c1, tbl.col2 c2, tbl.col3, 3.14 as val, 1.23, (tbl.col1 + tbl.col2) / tbl.col3 as colcalc, (1+2)*3 as numcalc";
        var item = SelectClauseParser.Parse(text);
        Monitor.Log(item);

        Assert.True(item.HasDistinctKeyword);
        Assert.NotNull(item.Top);
        Assert.Equal(8, item.Count);
    }
}