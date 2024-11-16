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
                a.id
                , a.value v1
                , a.value as v2
                , a.quantity * a.price as total_price
            from
                table as a
            """;

        var lexes = SelectQueryParser.Parse(text);

        output.WriteLine($"Text : {text}");

        Debugger.Print(output, lexes);
    }
}
