using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;
internal class QueryCommandMonitor
{
    private readonly ITestOutputHelper Output;

    public QueryCommandMonitor(ITestOutputHelper output)
    {
        Output = output;
    }

    public void Log(IQueryCommand arg)
    {
        var frm = new CommandFormatter();
        var bld = new CommandTextBuilder(frm);
        var sql = bld.Execute(arg.GetTokens(null));
        Output.WriteLine(sql);
        Output.WriteLine("--------------------");
        var len = 20;
        var indent = string.Empty;
        foreach (var item in arg.GetTokens(null))
        {
            var p = item.Parent == null ? "[null]" : item.Parent.Text;
            var s = item.Sender.GetType().Name;
            var l = item.Parents().Count();
            var r = (item.IsReserved) ? "reserved" : string.Empty;
            Output.WriteLine($"{(indent + item.Text).PadRight(len)} lv.{l} sender:{s.PadRight(15)}, parent:{p.PadRight(6)}, {r}");
        }
    }
}