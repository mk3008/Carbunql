using System.Text;
using Xunit.Abstractions;

namespace Carbunql.Analysis.Test;

public class StackOverFlowTest
{
	private readonly ITestOutputHelper Output;

	public StackOverFlowTest(ITestOutputHelper output)
	{
		Output = output;
	}

	[Fact]
	public void Union_50k()
	{
		var sb = new StringBuilder();
		sb.Append("select 1");
		for (int i = 0; i < 50000; i++)
		{
			sb.Append(" union all select 1");
		}

		var exception = Record.Exception(() =>
		{
			var sq = new SelectQuery(sb.ToString());
			sq.GetTokens();

			Output.WriteLine(sq.ToText());
		});

		Assert.Null(exception);
	}
}