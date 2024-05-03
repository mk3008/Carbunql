using Xunit.Abstractions;

namespace Carbunql.Annotation.Test;

public class QueryCommandMonitor
{
    private readonly ITestOutputHelper Output;

    public QueryCommandMonitor(ITestOutputHelper output)
    {
        Output = output;
    }

    public void Log(IQueryCommandable arg)
    {
        Output.WriteLine(arg.ToText());
        Output.WriteLine("--------------------");

        Log(arg.GetTokens(null));
    }

    public void Log(IEnumerable<Token> args)
    {
        var len = 20;
        var indent = string.Empty;
        var index = 0;
        foreach (var item in args)
        {
            var p = item.Parent == null ? "[null]" : item.Parent.Text;
            var s = item.Sender.GetType().Name;
            var l = item.Parents().Count();
            var r = item.IsReserved ? "reserved" : string.Empty;
            Output.WriteLine($"{index.ToString().PadLeft(3)} {(indent + item.Text).PadRight(len)} lv.{l} sender:{s.PadRight(15)}, parent:{p.PadRight(6)}, {r}");
            index++;
        }
    }
}