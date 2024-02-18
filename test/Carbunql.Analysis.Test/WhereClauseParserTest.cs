using Carbunql.Analysis.Parser;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class WhereClauseParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public WhereClauseParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void Default()
	{
		var text = @"tbl.col1 = 1 
and (tbl.col2 = 2 or tbl.col3 = 3) 
and (tbl.col2 = 2 and tbl.col3 = 3) 
and t1.c1 between 1 and 10";
		var item = WhereClauseParser.Parse(text);
		Monitor.Log(item);
	}

	[Fact]
	public void ParseNull()
	{
		var text = @"tbl.col1 is null and tbl.col2 is not null";
		var item = WhereClauseParser.Parse(text);
		Monitor.Log(item);

		var lst = item.GetTokens(null).ToList();
		Assert.Equal(12, lst.Count());

		Assert.Equal("col1", lst[3].Text);
		Assert.Equal("is", lst[4].Text);
		Assert.Equal("null", lst[5].Text);

		Assert.Equal("col2", lst[9].Text);
		Assert.Equal("is not", lst[10].Text);
		Assert.Equal("null", lst[11].Text);
	}

	[Fact]
	public void ParseIn()
	{
		var text = @"tbl.col1 in (1, 2, 3)";
		var item = WhereClauseParser.Parse(text);
		Monitor.Log(item);

		var lst = item.GetTokens(null).ToList();
		Assert.Equal(12, lst.Count());

		Assert.Equal("col1", lst[3].Text);
		Assert.Equal("in", lst[4].Text);
		Assert.Equal("(", lst[5].Text);
		Assert.Equal("1", lst[6].Text);
		Assert.Equal(",", lst[7].Text);

		Assert.Equal(")", lst[11].Text);
	}

	[Fact]
	public void ParseInSubQuery()
	{
		var text = @"tbl.col1 in (select id from table_b)";
		var item = WhereClauseParser.Parse(text);
		Monitor.Log(item);

		var lst = item.GetTokens(null).ToList();
		Assert.Equal(11, lst.Count());

		Assert.Equal("in", lst[4].Text);
		Assert.Equal("(", lst[5].Text);
		Assert.Equal("select", lst[6].Text);

		Assert.Equal(")", lst[10].Text);
	}
}
