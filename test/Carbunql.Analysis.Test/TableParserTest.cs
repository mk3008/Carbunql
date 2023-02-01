using Carbunql.Analysis.Parser;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class TableParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public TableParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void PhysicalTable()
	{
		var text = "schema_name.table_name";
		var v = TableParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(3, lst.Count);
	}

	[Fact]
	public void PhysicalTable2()
	{
		var text = "table_name";
		var v = TableParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
	}

	[Fact]
	public void ValuesTable()
	{
		var text = "(values (1,2.3,'a'), (4,5.6,'b'))";
		var v = TableParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(18, lst.Count);
	}
}