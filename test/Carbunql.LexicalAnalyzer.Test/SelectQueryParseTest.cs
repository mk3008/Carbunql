using Xunit.Abstractions;

namespace Carbunql.LexicalAnalyzer.Test;

public class SelectQueryParseTest(ITestOutputHelper output)
{
    private readonly ITestOutputHelper output = output;

    [Fact]
    public void Default()
    {
        var text = """
            select
                a.id,
                a.value,
                a.quantity * a.price as total_price
            from
                table as a
            """;

        var lexes = SelectQueryParser.Parse(text);

        output.WriteLine($"Text : {text}");
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"Lex[{index,3}] {lex.Value,-20}, Type: {lex.Type,-20}");
        }
    }
}
