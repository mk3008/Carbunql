using System.Data;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class QueryCommandMonitor
{
    public readonly ITestOutputHelper Output;

    public QueryCommandMonitor(ITestOutputHelper output)
    {
        Output = output;
    }

    public void Log(IQueryCommandable arg, bool exportTokens = true)
    {
        //var frm = new TokenFormatLogic();
        //var bld = new CommandTextBuilder(frm);
        //bld.Logger = (x) => Output.WriteLine(x);

        //var sql = bld.Execute(arg.GetTokens(null));
        Output.WriteLine("--------------------");
        Output.WriteLine(arg.ToText());

        if (!exportTokens) return;

        Output.WriteLine("--------------------");
        var len = 20;
        var indent = string.Empty;
        var index = 0;
        foreach (var item in arg.GetTokens(null))
        {
            var p = item.Parent == null ? "[null]" : item.Parent.Text;
            var s = item.Sender.GetType().Name;
            var l = item.Parents().Count();
            var r = item.IsReserved ? "reserved" : string.Empty;
            Output.WriteLine($"{index.ToString().PadLeft(3)} {(indent + item.Text).PadRight(len)} lv.{l} sender:{s.PadRight(15)}, parent:{p.PadRight(6)}, {r}");
            index++;
        }
    }

    public void Log(IEnumerable<IQuerySource> srouces)
    {
        var index = 0;
        foreach (var ds in srouces)
        {
            Output.WriteLine("--------------------");
            Output.WriteLine($"Index:{index}, Seq:{ds.Sequence}, Branch:{ds.SourceIndex}, Lv:{ds.Level}");
            Output.WriteLine($"Query:{ds.Query.ToOneLineText()}");
            Output.WriteLine($"Source:{ds.Source.Table.ToOneLineText()}");
            Output.WriteLine($"Table:{ds.GetTableFullName()}");
            Output.WriteLine($"Alias:{ds.Alias}");
            Output.WriteLine($"Columns:{string.Join(", ", ds.ColumnNames)}");
            index++;
        }
    }
}