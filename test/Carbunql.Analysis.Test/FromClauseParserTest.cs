using Carbunql.Analysis.Parser;
using Carbunql.Tables;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class FromClauseParserTest
{
    private readonly QueryCommandMonitor Monitor;

    public FromClauseParserTest(ITestOutputHelper output)
    {
        Monitor = new QueryCommandMonitor(output);
    }

    [Fact]
    public void Default()
    {
        var text = "public.table_a as a inner join table_b as b on a.id = b.id left join table_c as c on a.id = c.id";
        var item = FromClauseParser.Parse(text);
        Monitor.Log(item);

        Assert.Equal(2, item.Relations?.Count);

        var lst = item.GetPhysicalTables().ToList();
        Assert.Equal(3, lst.Count());
        Assert.Equal("public.table_a", lst[0].GetTableFullName());
        Assert.Equal("table_b", lst[1].GetTableFullName());
        Assert.Equal("table_c", lst[2].GetTableFullName());
    }

    [Fact]
    public void SubQuery()
    {
        var text = "(select a.id, a.val + 1 as val from table_a as a) as a1";
        var item = FromClauseParser.Parse(text);
        Monitor.Log(item);

        Assert.IsType<VirtualTable>(item.Root.Table);
        Assert.IsType<SelectQuery>(((VirtualTable)item.Root.Table).Query);

        var lst = item.GetPhysicalTables().ToList();
        Assert.Single(lst);
        Assert.Equal("table_a", lst[0].GetTableFullName());

        var lst2 = item.GetInternalQueries().SelectMany(x => x.GetSelectableTables()).ToList();

        Assert.Single(lst2);
        Assert.Equal("table_a", lst2[0].Table.GetTableFullName());
    }

    [Fact]
    public void ValuesQuery()
    {
        var text = "(values (1,2), (3,4)) as v(col1, col2)";
        var item = FromClauseParser.Parse(text);
        Monitor.Log(item);

        Assert.IsType<VirtualTable>(item.Root.Table);
        Assert.IsType<ValuesQuery>(((VirtualTable)item.Root.Table).Query);

        var lst = item.GetPhysicalTables().ToList();
        Assert.Empty(lst);
    }

    [Fact]
    public void FunctionQuery()
    {
        var text = "generate_series(2,4)";
        var item = FromClauseParser.Parse(text);
        Monitor.Log(item);

        Assert.IsType<FunctionTable>(item.Root.Table);
        var t = (FunctionTable)item.Root.Table;
        Assert.Equal("generate_series", t.Name);

        var lst = t.GetTokens(null).ToList();
        Assert.Equal(6, lst.Count);
        Assert.Equal("generate_series", lst[0].Text);
        Assert.Equal("(", lst[1].Text);
        Assert.Equal("2", lst[2].Text);
        Assert.Equal(",", lst[3].Text);
        Assert.Equal("4", lst[4].Text);
        Assert.Equal(")", lst[5].Text);

        var tables = item.GetPhysicalTables().ToList();
        Assert.Empty(tables);
    }
}