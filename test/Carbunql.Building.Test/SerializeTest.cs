using Xunit.Abstractions;

namespace Carbunql.Building.Test;

public partial class SerializeTest
{
    private ITestOutputHelper Output;

    public SerializeTest(ITestOutputHelper output)
    {
        Output = output;
    }

    private string TruncateControlString(string text)
    {
        return text.Replace("\r", "").Replace("\n", "").Replace(" ", "").ToLower();
    }
}
