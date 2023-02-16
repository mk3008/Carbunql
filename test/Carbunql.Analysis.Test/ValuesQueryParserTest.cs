using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class ValuesQueryParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public ValuesQueryParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void Default()
	{
		var text = @"
values
    (1,1),
    (2,2)
order by 1 desc 
limit 1";

		var sq = QueryParser.Parse(text);
		Monitor.Log(sq);

		Assert.Equal(17, sq.GetTokens().ToList().Count);
	}

	[Fact]
	public void ToSelectQuery()
	{
		var text = @"
values
    (1,1),
    (2,2)
union all
values
    (1,1),
    (2,2)
order by 1 desc 
limit 1";

		var q = QueryParser.Parse(text);
		var sq = q.GetOrNewSelectQuery();

		Monitor.Log(sq);

		Assert.Equal(48, sq.GetTokens().ToList().Count);
	}
}