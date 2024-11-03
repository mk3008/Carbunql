namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    private static HashSet<string> SpecialValueKeywords = new HashSet<string>
    {
        "true",
        "false",
        "now",
        "timestamp",
        "null",
        "current_timestamp",
        "current_date",
        "current_time",
        "epoch",
        "infinity",
        "-infinity",
        "today",
        "tomorrow",
        "yesterday",
        "allballs"
    };

    internal static bool TryParseSpecialValue(ReadOnlyMemory<char> memory, ref int position, out Lex lex)
    {
        lex = default;
        var start = position;

        // Ensure we are within bounds
        if (memory.Length < position)
        {
            return false;
        }

        foreach (var keyword in SpecialValueKeywords)
        {
            if (MemoryEqualsIgnoreCase(memory, position, keyword))
            {
                position += keyword.Length;
                lex = new Lex(memory, LexType.Value, start, position - start);
                return true;
            }
        }
        return false;
    }
}
