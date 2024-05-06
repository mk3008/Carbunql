namespace Carbunql;

[Obsolete]
public class CommandAttribute : Attribute
{
    public CommandAttribute(string text)
    {
        Text = text;
        FullText = text;
    }

    public CommandAttribute(string text, string full)
    {
        Text = text;
        FullText = full;
    }

    public string Text { get; init; }

    public string FullText { get; init; }
}