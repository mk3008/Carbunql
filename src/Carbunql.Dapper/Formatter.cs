namespace Carbunql.Dapper;

public static class Formatter
{
    public static FormatType Type { get; set; } = FormatType.Simple;
}

public enum FormatType
{
    Decoration,
    Simple
}
