using Carbunql.Analysis.Parser;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class CreateTableParseTest
{
	private readonly QueryCommandMonitor Monitor;

	public CreateTableParseTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	//	[Fact]
	//	public void Default()
	//	{
	//		var text = @"CREATE TABLE public.sale (
	//	sale_id bigserial NOT NULL,
	//	sale_date date NOT NULL,
	//	price int8 NOT NULL,
	//	created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
	//	CONSTRAINT sale_pkey PRIMARY KEY (sale_id)
	//)";
	//		var v = QueryParser.Parse(text);
	//		Monitor.Log(v);

	//		var lst = v.GetTokens().ToList();
	//		Assert.Equal(3, lst.Count);
	//	}
}