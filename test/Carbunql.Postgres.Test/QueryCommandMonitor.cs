using Carbunql;
using Carbunql.Postgres.Analysis;
using Xunit.Abstractions;

namespace Carbunql.Postgres.Test;

public class QueryCommandMonitor
{
	private readonly ITestOutputHelper Output;

	public QueryCommandMonitor(ITestOutputHelper output)
	{
		Output = output;
	}

	public void Log(IQueryable query)
	{
		if (SelectableTableParser.TryParse(query.Expression, out var from))
		{
			Output.WriteLine("From");
			if (from.Table != null) Output.WriteLine($"   Table : {from.Table.ToText()}");
			Output.WriteLine($"   Alias : {from.Alias}");
		}
		else
		{
			Output.WriteLine($"From : [NULL]");
		}
		Output.WriteLine("--------------------");

		var ctes = CommonTableInfoParser.Parse(query.Expression);
		foreach (var cte in ctes)
		{
			Output.WriteLine($"CTE[{ctes.IndexOf(cte)}] : {cte.Alias}");
		}
		Output.WriteLine("--------------------");

		var text = ExpressionReader.Analyze(query.Expression);
		Output.WriteLine(text);
		Output.WriteLine("--------------------");
	}

	public void Log(IQueryCommandable arg)
	{
		//var frm = new TokenFormatLogic();
		//var bld = new CommandTextBuilder(frm);
		//bld.Logger = (x) => Output.WriteLine(x);

		//var sql = bld.Execute(arg.GetTokens(null));
		Output.WriteLine(arg.ToText());
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
}