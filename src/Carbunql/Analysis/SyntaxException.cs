namespace Carbunql.Analysis;

/// <summary>
/// Represents an exception that is thrown when a syntax error occurs.
/// </summary>
public class SyntaxException : Exception
{
    /// <summary>
    /// Initializes a new instance of the SyntaxException class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SyntaxException(string message) : base(message) { }
}
