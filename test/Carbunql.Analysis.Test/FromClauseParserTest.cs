using Carbunql.Analysis.Parser;
using Carbunql.Core;
using Carbunql.Core.Clauses;
using Carbunql.Core.Tables;
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
    }

    [Fact]
    public void SubQuery()
    {
        var text = "(select a.id, a.val + 1 as val from table_a as a) as a1";
        var item = FromClauseParser.Parse(text);
        Monitor.Log(item);

        Assert.IsType<VirtualTable>(item.Root.Table);
        Assert.IsType<SelectQuery>(((VirtualTable)item.Root.Table).Query);
    }

    [Fact]
    public void ValuesQuery()
    {
        var text = "(values (1,2), (3,4)) as v(col1, col2)";
        var item = FromClauseParser.Parse(text);
        Monitor.Log(item);

        Assert.IsType<VirtualTable>(item.Root.Table);
        Assert.IsType<ValuesClause>(((VirtualTable)item.Root.Table).Query);
    }
}