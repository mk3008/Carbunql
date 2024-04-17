using System.Text;
using Xunit.Abstractions;

namespace Carbunql.DynamicQuery.Test;

public class DebugPrinter
{
    private readonly Action<string> Print;

    public DebugPrinter()
    {
        Print = Console.WriteLine;
    }

    public DebugPrinter(ITestOutputHelper output)
    {
        Print = output.WriteLine;
    }

    public DebugPrinter(Action<string> print)
    {
        Print = print;
    }

    public string Write(SelectQuery query)
    {
        return Write(query.ToCommand());
    }

    public string Write(QueryCommand cmd)
    {
        var sb = new StringBuilder();
        if (cmd.Parameters.Any())
        {
            sb.AppendLine("/*");
            foreach (var prm in cmd.Parameters)
            {
                sb.AppendLine($"    {prm.Key} = {prm.Value}");
            }
            sb.AppendLine("*/");
        }
        sb.AppendLine(cmd.CommandText);

        var s = sb.ToString().Replace("\r\n", "\n");
        Print(s);
        return s;
    }
}
