namespace Carbunql.LexicalAnalyzer;

public static partial class Lexer
{
    private static HashSet<string> SpecialValueWords = new HashSet<string>
    {
        "true",
        "false",
        "null",

        "current_timestamp",
        "current_date",
        "current_time",
        "timestamp",
        "now",

        "yesterday",
        "today",
        "tomorrow",

        "-infinity",
        "infinity",
        "allballs",
        "epoch",
    };

    private static bool IsSpacialValueWord(ReadOnlyMemory<char> memory, in int position, int length)
    {
        foreach (var keyword in SpecialValueWords.Where(x => x.Length == length))
        {
            if (MemoryEqualsIgnoreCase(memory, position, keyword))
            {
                return true;
            }
        }
        return false;
    }
}
