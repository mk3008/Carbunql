using Carbunql.Values;
using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class LiteralTest
{
	private readonly QueryCommandMonitor Monitor;

	public LiteralTest(ITestOutputHelper output)
	{
		Monitor = new QueryCommandMonitor(output);
	}

	[Fact]
	public void CommandText()
	{
		var v = new LiteralValue("text");

		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
		Assert.Equal("text", lst[0].Text);
	}

	[Fact]
	public void Text()
	{
		var v = new LiteralValue("'text'");

		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
		Assert.Equal("'text'", lst[0].Text);
	}

	[Fact]
	public void Numeric()
	{
		var v = new LiteralValue(1);

		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
		Assert.Equal("1", lst[0].Text);
	}

	[Fact]
	public void Null()
	{
		int? val = null;
		var v = new LiteralValue(val);

		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
		Assert.Equal("null", lst[0].Text);
	}

	[Fact]
	public void Bool()
	{
		var v = new LiteralValue(true);

		Monitor.Log(v);

		var lst = v.GetTokens().ToList();
		Assert.Single(lst);
		Assert.Equal("True", lst[0].Text);
	}
}