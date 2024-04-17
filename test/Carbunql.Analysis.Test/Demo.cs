using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class Demo
{
    public Demo(ITestOutputHelper output)
    {
        Output = output;
    }

    private readonly ITestOutputHelper Output;

    private void DebugPrint(QueryCommand cmd)
    {
        if (cmd.Parameters.Any())
        {
            Output.WriteLine("/*");
            foreach (var prm in cmd.Parameters)
            {
                Output.WriteLine($"    {prm.Key} = {prm.Value}");
            }
            Output.WriteLine("*/");
        }
        Output.WriteLine(cmd.CommandText);
    }

    [Fact]
    public void Select()
    {
        var text = @"
select a.column_1 as col1, a.column_2 as col2
from table_a as a
left join table_b as b on a.id = b.table_a_id
where b.table_a_id is null
";

        var q = new SelectQuery(text);
        DebugPrint(q.ToCommand());
    }
}
