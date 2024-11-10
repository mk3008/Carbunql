using Xunit.Abstractions;

namespace Carbunql.LexicalAnalyzer.Test;

public class ValueTest(ITestOutputHelper output)
{
    private readonly ITestOutputHelper output = output;

    [Fact]
    public void TestNumeric()
    {
        var text = "1234";
        var position = 0;
        Lex lex;
        var result = Lexer.TryParseNumericValue(text.AsMemory(), ref position, out lex);

        Assert.True(result);
        if (result)
        {
            Assert.Equal(text, lex.Value);
        }
    }
}
