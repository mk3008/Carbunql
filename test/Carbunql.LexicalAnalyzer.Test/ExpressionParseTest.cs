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

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("1", lexes[0].Value);
    }

    [Fact]
    public void WhiteSpaceSkip()
    {
        var text = " \t\r\n1";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("1", lexes[0].Value);
    }

    [Fact]
    public void LineCommentSkip()
    {
        var text = "--comment\n1";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("1", lexes[0].Value);
    }

    [Fact]
    public void BlockCommentSkip()
    {
        var text = "/*comment*/\n1";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("1", lexes[0].Value);
    }

    [Fact]
    public void HintCommentSkip()
    {
        var text = "/*+comment*/\n1";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("1", lexes[0].Value);
    }

    [Fact]
    public void EndOfSpace()
    {
        var text = "1 ";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("1", lexes[0].Value);
    }

    [Fact]
    public void EndOfNewLine()
    {
        var text = "1\n";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("1", lexes[0].Value);
    }

    [Fact]
    public void EndOfReturn()
    {
        var text = "1\r";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("1", lexes[0].Value);
    }

    [Fact]
    public void Numeric()
    {
        var text = "3.14 ";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("3.14", lexes[0].Value);
    }

    [Fact]
    public void SingleQuotedText()
    {
        var text = "'abc' ";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("'abc'", lexes[0].Value);
    }
}