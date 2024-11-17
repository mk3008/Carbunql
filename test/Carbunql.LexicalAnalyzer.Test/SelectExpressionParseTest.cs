using Xunit.Abstractions;

namespace Carbunql.LexicalAnalyzer.Test;

public class SelectExpressionParseTest(ITestOutputHelper output)
{
    private readonly ITestOutputHelper output = output;

    [Theory]
    [InlineData("as name,", true, "name")]
    [InlineData("name,", true, "name")]
    [InlineData("name ", true, "name")]
    [InlineData(",", false, "")]
    [InlineData("[n a m e],", true, "[n a m e]")]
    [InlineData("\"n a m e\",", true, "\"n a m e\"")]
    [InlineData("`n a m e`,", true, "`n a m e`")]
    [InlineData("from", false, "")]
    public void ExpressionName(string text, bool hasAlias, string expectedValue)
    {
        output.WriteLine($"Text: {text}");

        int position = 0;
        bool result = Lexer.TryParseExpressionName(text.AsMemory(), position, out var lex, out position);

        if (hasAlias)
        {
            output.WriteLine($"Alias: {lex.Value}");
            Assert.True(result, $"Expected to successfully parse '{text}' as a valid column access with alias.");
            Assert.Equal(expectedValue, lex.Value);
        }
        else
        {
            output.WriteLine("Alias: [none]");
            Assert.False(result, $"Expected to fail parsing '{text}' as a valid column access without alias.");
        }
    }
}
