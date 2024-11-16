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

        Debugger.Print(output, lexes);

        Assert.Equal("--", lexes[0].Value);
        Assert.Equal("comment", lexes[1].Value);
    }

    [Fact]
    public void BlockComment()
    {
        var text = "/*comment*/1";
        var lexes = text.AsMemory().ParseUntilNonComment(0).ToList();

        output.WriteLine($"Text : {text}");

        Debugger.Print(output, lexes);

        var lst = lexes.ToList();
        Assert.Equal("/*", lst[0].Value);
        Assert.Equal("comment", lst[1].Value);
        Assert.Equal("*/", lst[2].Value);
    }
}
