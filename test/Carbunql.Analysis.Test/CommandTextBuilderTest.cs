using Carbunql.Core;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class CommandTextBuilderTest
{
    private readonly ITestOutputHelper Output;

    public CommandTextBuilderTest(ITestOutputHelper output)
    {
        Output = output;
    }

    [Fact]
    public void Default()
    {
        var sql = @"select
    a.table_a_id as id,
    3.14 as val,
    (a.val + b.val) * 2 as calc, 
    b.table_b_id,
    c.table_c_id
from 
    table_a a
    inner join table_b b on a.table_a_id = b.table_a_id and b.visible = true
    left join table_c c on a.table_a_id = c.table_a_id";

        var sq = QueryParser.Parse(sql);
        var frm = new CommandFormatter();
        var b = new CommandTextBuilder(frm);

        var formatsql = b.Execute(sq.GetTokens(null));
        Output.WriteLine(formatsql);
    }
}
