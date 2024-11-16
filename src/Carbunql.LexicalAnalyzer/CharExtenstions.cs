namespace Carbunql.LexicalAnalyzer;

internal static class CharExtenstions
{
    /// <summary>
    /// Defines a set of characters considered as symbols that terminate an identifier.
    /// </summary>
    private static readonly HashSet<char> Symbols = new HashSet<char>
    {
        '+', '-', '*', '/', '%', // Arithmetic operators
        '(', ')', '[', ']', '{', '}', // Brackets and braces
        '~', '@', '#', '$', '^', '&', // Special symbols
        '!', '?', ':', ';', ',', '.', '<', '>', '=', '|', '\\', // Other symbols
        '`', '"', '\'' // Quotation marks
    };

    private static readonly HashSet<char> WhiteSpaces = new HashSet<char>
    {
        ' ', '\t', '\r', '\n',
    };

    private static readonly Dictionary<char, char> ValueEscapePairs = new Dictionary<char, char>
    {
        { '"', '"' }, // ダブルクォート
        { '[', ']' }, // 角括弧
        { '`', '`' }  // バッククォート
    };

    public static bool TryGetDbmsValueEscapeChar(this char c, out char closeChar)
    {
        return ValueEscapePairs.TryGetValue(c, out closeChar);
    }

    public static bool IsWhiteSpace(this char c)
    {
        return WhiteSpaces.Contains(c);
    }

    public static bool IsSymbols(this char c)
    {
        return Symbols.Contains(c);
    }
}
