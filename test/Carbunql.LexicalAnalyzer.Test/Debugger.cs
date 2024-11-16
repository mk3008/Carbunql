using Xunit.Abstractions;

namespace Carbunql.LexicalAnalyzer.Test;

internal static class Debugger
{
    public static void Print(ITestOutputHelper output, IEnumerable<Lex> lexes)
    {
        var count = 0;
        foreach (var (lex, index) in lexes.Select((lex, index) => (lex, index)))
        {
            output.WriteLine($"[{index,3}][{lex.Type,-20}] {lex.Value}");
            count++;
        }

        output.WriteLine($"Count : {count}");
    }
}
