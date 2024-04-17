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
        sb.Append("select 1,2,3,4,5,6,7,8,9,10");
        for (int i = 0; i < 50000; i++)
        {
            sb.Append(" union all select 1,2,3,4,5,6,7,8,9,10");
        }

        var exception = Record.Exception(() =>
        {
            var sq = new SelectQuery(sb.ToString());
            sq.GetTokens();

            Output.WriteLine(sq.ToText());
        });

        Assert.Null(exception);
    }

    [Fact]
    public void Values_50k()
    {
        var sb = new StringBuilder();
        sb.Append("values (1,2,3,4,5,6,7,8,9,10)");
        for (int i = 0; i < 50000; i++)
        {
            sb.Append(", (1,2,3,4,5,6,7,8,9,10)");
        }

        var exception = Record.Exception(() =>
        {
            var vq = new ValuesQuery(sb.ToString());
            vq.GetTokens();

            Output.WriteLine(vq.ToText());
        });

        Assert.Null(exception);
    }
}