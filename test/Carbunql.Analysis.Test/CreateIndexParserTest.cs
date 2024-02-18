using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class CreateIndexParserTest
{
	private readonly QueryCommandMonitor Monitor;

	public CreateIndexParserTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void Default()
	{
		var text = @"CREATE INDEX index_name ON table_name (
	column_name
)";
		var v = CreateIndexQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(7, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}

	[Fact]
	public void CompositeKey()
	{
		var text = @"CREATE INDEX index_name ON table_name (
	column1,
	column2
)";
		var v = CreateIndexQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(9, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}

	[Fact]
	public void Condition()
	{
		var text = @"CREATE INDEX index_name ON table_name (
	column_name
)
WHERE
	column_name IS NOT NULL";
		var v = CreateIndexQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(11, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}

	[Fact]
	public void UniqueKey()
	{
		var text = @"CREATE UNIQUE INDEX index_name ON table_name (
	column_name
)";
		var v = CreateIndexQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(7, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}


	[Fact]
	public void Nameless()
	{
		var text = @"CREATE INDEX ON my_table (
	name
)";
		var v = CreateIndexQueryParser.Parse(text);
		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Equal(6, lst.Count);
		Assert.Equal(text, v.ToCommand().CommandText, true, true, true);
	}
}