using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class EscapedIdentiferTest
{
	private readonly QueryCommandMonitor Monitor;

	public EscapedIdentiferTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void Postgres()
	{
		var expect = @"SELECT
	1 AS ""a b""";
		var sq = new SelectQuery(expect);
		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(4, lst.Count);

		Assert.Equal(expect, sq.ToText(), true, true, true);
	}

	[Fact]
	public void Oracle()
	{
		var expect = @"SELECT
	1 AS ""a b""
FROM
	dual";
		var sq = new SelectQuery(expect);
		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(6, lst.Count);

		Assert.Equal(expect, sq.ToText(), true, true, true);
	}

	[Fact]
	public void MySql()
	{
		var expect = @"SELECT
	1 AS `a b`";
		var sq = new SelectQuery(expect);
		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(4, lst.Count);

		Assert.Equal(expect, sq.ToText(), true, true, true);
	}

	[Fact]
	public void SQLServer()
	{
		var expect = @"SELECT
	1 AS [a b]";
		var sq = new SelectQuery(expect);
		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(4, lst.Count);

		Assert.Equal(expect, sq.ToText(), true, true, true);
	}

	[Fact]
	public void SingleQuoteEscape()
	{
		var expect = @"SELECT
	'It''s raining' AS ""column name""";
		var sq = new SelectQuery(expect);
		Monitor.Log(sq);

		var lst = sq.GetTokens().ToList();
		Assert.Equal(4, lst.Count);

		Assert.Equal(expect, sq.ToText(), true, true, true);
	}
}