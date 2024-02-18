using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class QueryParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public QueryParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void Select()
	{
		var text = @"
select
    *
from
    table_a";

		var item = QueryParser.Parse(text);
		Monitor.Log(item);
	}

	[Fact]
	public void Values()
	{
		var text = @"
values
    (1,1),
    (2,2)
order by 1 desc 
limit 1";

		var item = QueryParser.Parse(text);
		Monitor.Log(item);
	}
}