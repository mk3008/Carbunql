using Xunit.Abstractions;

namespace Carbunql.LexicalAnalyzer.Test;

public class ExpressionParseTest(ITestOutputHelper output)
{
    private readonly ITestOutputHelper output = output;

    [Fact]
    public void Default()
    {
        var text = "1";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Assert.Equal("1", lexes[0].Value);
    }

    [Fact]
    public void WhiteSpaceSkip()
    {
        var text = " \t\r\n1";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Equal("1", lexes[0].Value);
    }

    [Fact]
    public void LineCommentSkip()
    {
        var text = " --comment\n1";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Equal("1", lexes[0].Value);
    }

    [Theory]
    [InlineData("/*comment*/\n1", "1")]
    [InlineData("/*+comment*/\n1", "1")]
    public void CommentSkip(string text, string expectedValue)
    {
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Single(lexes);
        Assert.Equal(expectedValue, lexes[0].Value);
    }

    [Fact]
    public void EndOfSpace()
    {
        var text = " 1 ";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Equal("1", lexes[0].Value);
    }

    [Theory]
    [InlineData("1\n", "1")]
    [InlineData("1\r", "1")]
    public void EndOfLineVariations(string text, string expectedValue)
    {
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Single(lexes);
        Assert.Equal(expectedValue, lexes[0].Value);
    }


    [Theory]
    [InlineData("42", "42")]
    [InlineData("3.14", "3.14")]
    [InlineData("- 3.14", "- 3.14")]
    public void NumericValues(string text, string expectedValue)
    {
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Single(lexes);
        Assert.Equal(expectedValue, lexes[0].Value);
    }

    [Theory]
    [InlineData("'abc'", "'abc'")]
    [InlineData("'abc''s'", "'abc''s'")]
    public void SingleQuotedText(string text, string expectedValue)
    {
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Single(lexes);
        Assert.Equal(expectedValue, lexes[0].Value);
    }

    [Theory]
    [InlineData("value", new[] { "value" })]
    [InlineData("table.value", new[] { "table", "value" })]
    [InlineData("schema.table.value", new[] { "schema", "table", "value" })]
    public void ColumnNames(string text, string[] expectedValues)
    {
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Equal(expectedValues.Length, lexes.Count);

        for (int i = 0; i < expectedValues.Length; i++)
        {
            Assert.Equal(expectedValues[i], lexes[i].Value);
        }
    }

    [Fact]
    public void ArithmeticOperations()
    {
        var text = " 1+2-3*4/5 ";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Equal("1", lexes[0].Value);
        Assert.Equal("+", lexes[1].Value);
        Assert.Equal("2", lexes[2].Value);
        Assert.Equal("-", lexes[3].Value);
        Assert.Equal("3", lexes[4].Value);
        Assert.Equal("*", lexes[5].Value);
        Assert.Equal("4", lexes[6].Value);
        Assert.Equal("/", lexes[7].Value);
        Assert.Equal("5", lexes[8].Value);
    }

    [Theory]
    [InlineData("a = 0", new[] { "a", "=", "0" })]
    [InlineData("b != 0", new[] { "b", "!=", "0" })]
    [InlineData("c <> 0", new[] { "c", "<>", "0" })]
    [InlineData("0 < d", new[] { "0", "<", "d" })]
    [InlineData("0 <= f", new[] { "0", "<=", "f" })]
    [InlineData("g < 0", new[] { "g", "<", "0" })]
    [InlineData("h <= 0", new[] { "h", "<=", "0" })]
    [InlineData("i is null", new[] { "i", "is", "null" })]
    [InlineData("j is not null", new[] { "j", "is not", "null" })]
    public void Operators(string text, string[] expectedValues)
    {
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Equal(expectedValues.Length, lexes.Count);

        for (int i = 0; i < expectedValues.Length; i++)
        {
            Assert.Equal(expectedValues[i], lexes[i].Value);
        }
    }

    [Theory]
    [InlineData("a=1 and b=2", new[] { "a", "=", "1", "and", "b", "=", "2" })]
    [InlineData("a=1 or b=2", new[] { "a", "=", "1", "or", "b", "=", "2" })]
    public void LogicalOperators(string text, string[] expectedValues)
    {
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Equal(expectedValues.Length, lexes.Count);

        for (int i = 0; i < expectedValues.Length; i++)
        {
            Assert.Equal(expectedValues[i], lexes[i].Value);
        }
    }

    [Theory]
    [InlineData("a + b", new[] { "a", "+", "b" })]
    [InlineData("a - b", new[] { "a", "-", "b" })]
    [InlineData("a * b", new[] { "a", "*", "b" })]
    [InlineData("a / b", new[] { "a", "/", "b" })]
    [InlineData("(a + b) * c", new[] { "(", "a", "+", "b", ")", "*", "c" })]
    [InlineData("x * (y - z)", new[] { "x", "*", "(", "y", "-", "z", ")" })]
    [InlineData("((a + b) * c) / d", new[] { "(", "(", "a", "+", "b", ")", "*", "c", ")", "/", "d" })]
    public void ArithmeticOperators(string text, string[] expectedValues)
    {
        output.WriteLine($"Text: {text}");

        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Equal(expectedValues.Length, lexes.Count);

        for (int i = 0; i < expectedValues.Length; i++)
        {
            Assert.Equal(expectedValues[i], lexes[i].Value);
        }
    }

    [Theory]
    [InlineData("a.value", new[] { "a", "value" })]
    [InlineData("table.column", new[] { "table", "column" })]
    [InlineData("schema.table.column", new[] { "schema", "table", "column" })]
    [InlineData("(a.value + b.value) * c.value", new[] { "(", "a", "value", "+", "b", "value", ")", "*", "c", "value" })]
    [InlineData("x.value * (y.value - z.value)", new[] { "x", "value", "*", "(", "y", "value", "-", "z", "value", ")" })]
    [InlineData("((a.value + b.value) * c.value) / d.value", new[] { "(", "(", "a", "value", "+", "b", "value", ")", "*", "c", "value", ")", "/", "d", "value" })]
    [InlineData("a.*", new[] { "a", "*" })]
    [InlineData("a.value1", new[] { "a", "value1" })]
    [InlineData("a.total_value", new[] { "a", "total_value" })]
    [InlineData("a.\"total value\"", new[] { "a", "\"total value\"" })]
    [InlineData("\"table a\".\"total value\"", new[] { "\"table a\"", "\"total value\"" })]
    [InlineData("\"table\"\"a\".\"total\"\"value\"", new[] { "\"table\"\"a\"", "\"total\"\"value\"" })]
    public void ColumnAccess(string text, string[] expectedValues)
    {
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Equal(expectedValues.Length, lexes.Count);

        for (int i = 0; i < expectedValues.Length; i++)
        {
            Assert.Equal(expectedValues[i], lexes[i].Value);
        }
    }

    [Theory]
    [InlineData("count(*)", new[] { "count", "(", "*", ")" })]
    [InlineData("sum(a.value)", new[] { "sum", "(", "a", "value", ")" })]
    [InlineData("cast(a.value as text)", new[] { "cast", "(", "a", "value", "as", "text", ")" })]
    [InlineData("a.value::text", new[] { "a", "value", "::", "text" })]
    public void TestFunction(string text, string[] expectedValues)
    {
        // type‚ª—ˆ‚é‚±‚Æ‚ª‚ ‚é

        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        Debugger.Print(output, lexes);

        Assert.Equal(expectedValues.Length, lexes.Count);

        for (int i = 0; i < expectedValues.Length; i++)
        {
            Assert.Equal(expectedValues[i], lexes[i].Value);
        }
    }
}