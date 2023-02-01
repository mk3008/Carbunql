using Carbunql.Analysis.Parser;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class SelectableTableParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public SelectableTableParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void PhysicalTable()
	{
		var text = "schema_name.table_name as t";
		var v = SelectableTableParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(5, lst.Count);
	}

	[Fact]
	public void PhysicalTable2()
	{
		var text = "table_name as t";
		var v = SelectableTableParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(3, lst.Count);
	}


	[Fact]
	public void PhysicalTable3()
	{
		var text = "table_name as table_name";
		var v = SelectableTableParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
	}

	[Fact]
	public void ValuesTable()
	{
		var text = "(values (1,2.3,'a'), (4,5.6,'b')) as t(a, b, c)";
		var v = SelectableTableParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(27, lst.Count);
	}

	[Fact]
	public void FunctionTable()
	{
		var text = "generate_series(0,14,7) AS s(a)";
		var v = SelectableTableParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(13, lst.Count);
	}

	[Fact]
	public void SubQuery()
	{
		var text = "(select a.id, a.val from table as a) as b";
		var v = SelectableTableParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(16, lst.Count);
	}
}