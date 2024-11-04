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
        var text = " --comment\n1";
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
        var text = " /*comment*/\n1";
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
        var text = " /*+comment*/\n1";
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
        var text = " 1 ";
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
        var text = " 1\n";
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
        var text = " 1\r";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("1", lexes[0].Value);
    }

    [Fact]
    public void UnsignedNumeric()
    {
        var text = " 3.14 ";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("3.14", lexes[0].Value);
    }

    [Fact]
    public void SignedNumeric()
    {
        var text = " +   3.14 ";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("+ 3.14", lexes[0].Value);
    }

    [Fact]
    public void SingleQuotedText()
    {
        var text = " 'abc' ";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("'abc'", lexes[0].Value);
    }

    [Fact]
    public void SingleQuotedTextWithEscape()
    {
        var text = " 'abc''s' ";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("'abc''s'", lexes[0].Value);
    }

    [Fact]
    public void Column()
    {
        var text = " value ";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("value", lexes[0].Value);
    }

    [Fact]
    public void TableColumn()
    {
        var text = " table.value ";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Type: {lex.Type}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("table", lexes[0].Value);
        Assert.Equal(".", lexes[1].Value);
        Assert.Equal("value", lexes[2].Value);
    }

    [Fact]
    public void SchemaTableColumn()
    {
        var text = " schema.table.value ";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Type: {lex.Type}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("schema", lexes[0].Value);
        Assert.Equal(".", lexes[1].Value);
        Assert.Equal("table", lexes[2].Value);
        Assert.Equal(".", lexes[3].Value);
        Assert.Equal("value", lexes[4].Value);
    }

    [Fact]
    public void ArithmeticOperations()
    {
        var text = " 1+2-3*4/5 ";
        var lexes = Lexer.ReadExpressionLexes(text.AsMemory(), 0).ToList();

        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Type: {lex.Type}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

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
}