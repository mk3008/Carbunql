using Xunit.Abstractions;

namespace Carbunql.LexicalAnalyzer.Test;

public class CommentTest(ITestOutputHelper output)
{
    private readonly ITestOutputHelper output = output;

    [Fact]
    public void LineComment()
    {
        var text = "--comment\n1";
        var lexes = text.AsMemory().ParseUntilNonComment(0).ToList();

        output.WriteLine($"Text : {text}");
        output.WriteLine($"Count : {lexes.Count()}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        Assert.Equal("--", lexes[0].Value);
        Assert.Equal("comment", lexes[1].Value);
    }

    [Fact]
    public void BlockComment()
    {
        var text = "/*comment*/1";
        var lexes = text.AsMemory().ParseUntilNonComment(0).ToList();

        output.WriteLine($"Text : {text}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Index: {index}, Position: {lex.Position}, Length: {lex.Length},  Value: {lex.Value}");
        }

        var lst = lexes.ToList();
        Assert.Equal("/*", lst[0].Value);
        Assert.Equal("comment", lst[1].Value);
        Assert.Equal("*/", lst[2].Value);
    }
}
