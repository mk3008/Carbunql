namespace Carbunql.LexicalAnalyzer;

public readonly struct Lex
{
    public Lex(ReadOnlyMemory<char> memory, LexType type, int position, int length)
    {
        Memory = memory;
        Type = type;
        Position = position;
        Length = length;
        EndPosition = Position + Length;
        cachedValue = null;
    }

    public Lex(ReadOnlyMemory<char> memory, LexType type, int position, int length, string value)
    {
        Memory = memory;
        Type = type;
        Position = position;
        Length = length;
        EndPosition = Position + Length;
        cachedValue = value;
    }

    public Lex(ReadOnlyMemory<char> memory, LexType type, int position, int length, int endPosition)
    {
        Memory = memory;
        Type = type;
        Position = position;
        Length = length;
        EndPosition = endPosition;
        cachedValue = null;
    }

    public readonly ReadOnlyMemory<char> Memory { get; }
    public LexType Type { get; }
    public int Position { get; }
    public int Length { get; }
    public int EndPosition { get; }

    private readonly string? cachedValue;

    public string Value => GetValue();

    private string GetValue()
    {
        if (cachedValue == null)
        {
            if (Position + Length > Memory.Length)
                throw new ArgumentOutOfRangeException(nameof(Length), "Position and Length exceed memory bounds.");

            return Memory.Slice(Position, Length).ToString();
        }
        return cachedValue;
    }
}


