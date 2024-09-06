using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public class CopyTest
{
    private ITestOutputHelper Output;

    public CopyTest(ITestOutputHelper output)
    {
        Output = output;
    }

    [Fact]
    public void DeepCopy()
    {
        var sq = SelectQuery.Parse("select a.column_1 as c1 from table_a as a");
        var actual = sq.DeepCopy();

        Assert.Equal(sq.ToText().ToValidateText(), actual!.ToText().ToValidateText());
        Assert.NotEqual(sq, actual);
    }
}
