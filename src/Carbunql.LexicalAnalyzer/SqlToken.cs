namespace Carbunql.LexicalAnalyzer;

public struct SqlToken
{
    public SqlToken(TokenType type, ReadOnlyMemory<char> value, int startPosition, int endPosition, int parenthesisLevel, int bracketLevel, string formattedValue)
    {
        Type = type;
        Value = value;
        StartPosition = startPosition;
        EndPosition = endPosition;
        ParenthesisLevel = parenthesisLevel;
        BracketLevel = bracketLevel;
        FormattedValue = formattedValue;
    }

    public SqlToken(TokenType type, ReadOnlyMemory<char> value, int startPosition, int endPosition, int parenthesisLevel, int bracketLevel)
    {
        Type = type;
        Value = value;
        StartPosition = startPosition;
        EndPosition = endPosition;
        ParenthesisLevel = parenthesisLevel;
        BracketLevel = bracketLevel;
        FormattedValue = new string(Value.Span).Trim();
    }

    public TokenType Type { get; }
    public ReadOnlyMemory<char> Value { get; }
    public int StartPosition { get; }
    public int EndPosition { get; }
    public int ParenthesisLevel { get; }  // () level
    public int BracketLevel { get; }      // [] level
    public string FormattedValue { get; }
}

public enum TokenType : byte
{
    Keyword,

    LeftParen,
    RightParen,

    LeftBracket,
    RightBracket,

    Identifier,
    Value,

    Operator,

    LineCommentStart,     // 一行コメント（例: -- コメント）

    BlockCommentStart, // ブロックコメント開始（例: /* コメント */）
    BlockCommentEnd,   // ブロックコメント終了

    HintCommentStart,
    HintCommentEnd,
    Comment,

    Dot,
    Comma,
}
