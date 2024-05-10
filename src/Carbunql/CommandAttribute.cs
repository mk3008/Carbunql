namespace Carbunql;

/// <summary>
/// Specifies that a command attribute is obsolete.
/// </summary>
[Obsolete]
public class CommandAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAttribute"/> class with the specified text.
    /// </summary>
    /// <param name="text">The text of the command.</param>
    public CommandAttribute(string text)
    {
        Text = text;
        FullText = text;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAttribute"/> class with the specified text and full text.
    /// </summary>
    /// <param name="text">The text of the command.</param>
    /// <param name="full">The full text of the command.</param>
    public CommandAttribute(string text, string full)
    {
        Text = text;
        FullText = full;
    }

    /// <summary>
    /// Gets the text of the command.
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// Gets the full text of the command.
    /// </summary>
    public string FullText { get; init; }
}
