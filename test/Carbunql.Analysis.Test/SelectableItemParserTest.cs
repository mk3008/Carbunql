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

        var lst = item.GetTokens().ToList();
        Assert.Single(lst);
    }

    [Fact]
    public void NotTableColumnAlias()
    {
        var text = "3.14 as val";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(3, lst.Count);
    }

    [Fact]
    public void TableColumn()
    {
        var text = "t.col";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(3, lst.Count);
    }

    [Fact]
    public void TableColumnAliasRedundant()
    {
        var text = "t.col as col";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(3, lst.Count);
    }

    [Fact]
    public void TableColumnAlias()
    {
        var text = "t.col as col1";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(5, lst.Count);
    }

    [Fact]
    public void TableColumnAlias1()
    {
        var text = "t.col col1";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(5, lst.Count);
    }

    [Fact]
    public void BreakToken()
    {
        var text = "t.col ,";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(3, lst.Count);
    }

    [Fact]
    public void RowNumberTest()
    {
        var text = "row_number() over (partition by d.tax_rate order by d.raw_tax % 1 desc, d.line_id) as val";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(23, lst.Count);
    }

    [Fact]
    public void Calc()
    {
        var text = "(tbl.col1 + tbl.col2) / tbl.col3 as colcalc";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(15, lst.Count);
    }

    [Fact]
    public void Bracket()
    {
        var text = "(1) as 1";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(5, lst.Count);
    }

    [Fact]
    public void BracketNest()
    {
        var text = "(((1))) as 1";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(9, lst.Count);
    }

    [Fact]
    public void Function()
    {
        var text = "sum(a.val) as val";
        var item = SelectableItemParser.Parse(text);
        Monitor.Log(item);

        var lst = item.GetTokens().ToList();
        Assert.Equal(8, lst.Count);
    }

    [Fact]
    public void FunctionIssue198()
    {
        var text = "greatest( (1+1)::int + 2 ) as v2";
        var v = SelectableItemParser.Parse(text);
        Monitor.Log(v);

        var lst = v.GetTokens().ToList();
        Assert.Equal(14, lst.Count);
    }

    //[Fact]
    //public void FunctionIssue198_Case()
    //{
    //	var text = "(1+1)::int";
    //	var v = CastValueParser.Parse(text);
    //	Monitor.Log(v);

    //	var lst = v.GetTokens().ToList();
    //	Assert.Equal(14, lst.Count);
    //}
}